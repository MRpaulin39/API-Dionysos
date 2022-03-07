using DIONYSOS.API.Context;
using DIONYSOS.API.Data.Models;
using DIONYSOS.API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace DIONYSOS.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Administrator")]
    [Route("api/products")]
    [Produces("application/json")]
    [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(EmptyResult), Description = "Veuillez vous authentifier à l'API")]
    [SwaggerResponse(HttpStatusCode.Forbidden, typeof(EmptyResult), Description = "Vous n'avez pas les privillèges nécessaires")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class ProductsController : ControllerBase
    {
        //Ajout du contructeur puis injection du context pour la connexion à la BDD
        private readonly DionysosContext _context;

        public ProductsController(DionysosContext context)
        {
            _context = context;
        }


        #region HTTP GET

        //Récupération de tous les produits
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ReadProductViewModels), Description = "La récupération de tous les produits avec le fournisseur associé a été un succès")]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(EmptyResult), Description = "La table produits est vide")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<ReadProductViewModels>> GetProducts()
        {
            //Si la table est vide
            if (!_context.Product.Any())
            {
                return NoContent();

            }

            var ProductSupplierLink = _context.Product
                .Join(
                    _context.Supplier,
                    Product => Product.Supplier.Id,
                    Supplier => Supplier.Id,
                    (Product, Supplier) => new ReadProductViewModels()
                    {
                        Id = Product.Id,
                        Name = Product.Name,
                        BarCode = Product.BarCode,
                        Price = Product.Price,
                        Description = Product.Description,
                        ImagePathFile = Product.ImagePathFile,
                        State = Product.State,
                        Quantity = Product.Quantity,
                        QuantityMax = Product.QuantityMax,
                        Site = Product.Site,
                        OrderAuto = Product.OrderAuto,
                        DayOrderAuto = Product.DayOrderAuto,
                        SupplierId = Product.Supplier.Id,
                        SupplierName = Supplier.Name

                    }
                ).ToList();

            return Ok(ProductSupplierLink);
        }

        //Récupération d'un produit en fonction de l'ID
        [HttpGet("{productId}", Name = "GetProduct")]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ReadProductViewModels), Description = "La récupération du produit avec le fournisseur associé a été un succès")]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(EmptyResult), Description = "La table produits est vide")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID du produit renseigné n'est pas connu de la base de données")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ReadProductViewModels> GetProductById(int productId)
        {
            //Si la table est vide
            if (!_context.Product.Any())
            {
                return NoContent();

            }

            //Vérification si le produit avec l'id renseigné existe
            var productToFind = _context.Product.Find(productId);
            if (productToFind == null)
            {
                return NotFound();
            }

            var ProductSupplierLink = _context.Product
                .Where(p => p.Id == productId)
                .Join(
                    _context.Supplier,
                    Product => Product.Supplier.Id,
                    Supplier => Supplier.Id,
                    (Product, Supplier) => new ReadProductViewModels()
                    {
                        Id = Product.Id,
                        Name = Product.Name,
                        BarCode = Product.BarCode,
                        Price = Product.Price,
                        Description = Product.Description,
                        ImagePathFile = Product.ImagePathFile,
                        State = Product.State,
                        Quantity = Product.Quantity,
                        QuantityMax = Product.QuantityMax,
                        Site = Product.Site,
                        OrderAuto = Product.OrderAuto,
                        DayOrderAuto = Product.DayOrderAuto,
                        SupplierId = Product.Supplier.Id,
                        SupplierName = Supplier.Name

                    }
                ).Single();

            return Ok(ProductSupplierLink);
        }

        #endregion

        //ToDo : Ajouter une vérification sur produits pas déjà existant

        #region HTTP POST

        //Permet de rajouter un produit avec l'id du fournisseur
        [HttpPost("suppliers/{supplierId}")]
        [SwaggerResponse(HttpStatusCode.Created, typeof(WriteProductViewModels), Description = "La création du produit a été un succès")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(WriteProductViewModels), Description = "Le fichier Json est incorrect")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "Une ou plusieurs valeurs dépassent le nombre de caractère autorisé")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID du fournisseur renseigné est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteProductViewModels> CreateProduct(int supplierId, WriteProductViewModels product)
        {
            //On vérifie que le JSON reçu est conforme au WriteModels
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            Product finalProduct = new Product();

            //Récupératation de la valeur de OrderAuto
            var OrderAutoValue = _context.Product
                            .OrderBy(p => p.Id)
                            .FirstOrDefault();

            //Vérification 
            var supplier = _context.Supplier.Find(supplierId);
            if (supplier == null)
            {
                return NotFound("L'ID du fournisseur entrée n'est pas correct");
            }

            finalProduct.Supplier = supplier;
            finalProduct.Name = product.Name;
            finalProduct.BarCode = product.BarCode;
            finalProduct.Price = product.Price;
            finalProduct.Description = product.Description;
            finalProduct.ImagePathFile = product.ImagePathFile;
            finalProduct.State = product.State;
            finalProduct.Quantity = product.Quantity;
            finalProduct.QuantityMax = product.QuantityMax;
            finalProduct.Site = product.Site;
            finalProduct.OrderAuto = OrderAutoValue.OrderAuto;
            finalProduct.DayOrderAuto = product.DayOrderAuto;


            _context.Product.Add(finalProduct);
            _context.SaveChanges();

            return CreatedAtRoute("GetProduct", new { productId = finalProduct.Id }, finalProduct);
        }

        #endregion

        #region HTTP PUT

        [HttpPut("{productId}/suppliers/{supplierId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(WriteProductViewModels), Description = "La mise à jour du produit a été un succès")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(WriteProductViewModels), Description = "Le fichier Json est incorrect")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "Une ou plusieurs valeurs dépassent le nombre de caractère autorisé")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID du produit ou du fournisseur renseigné est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteProductViewModels> UpdateProduct(int productId, int supplierId, WriteProductViewModels product)
        {
            //On vérifie que le JSON reçu est conforme au ViewModels
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var OrderAutoValue = _context.Product
                            .OrderBy(p => p.Id)
                            .FirstOrDefault();

            var productFromDataBase = _context.Product.Single(p => p.Id == productId);
            if (product == null)
            {
                return NotFound("L'ID du produit envoyé n'est pas correct");
            }

            var supplier = _context.Supplier.Find(supplierId);
            if (supplier == null)
            {
                return NotFound("L'ID du fournisseur entré n'est pas correct");
            }

            productFromDataBase.Name = product.Name;
            productFromDataBase.BarCode = product.BarCode;
            productFromDataBase.Price = product.Price;
            productFromDataBase.Description = product.Description;
            productFromDataBase.ImagePathFile = product.ImagePathFile;
            productFromDataBase.State = product.State;
            productFromDataBase.Quantity = product.Quantity;
            productFromDataBase.QuantityMax = product.QuantityMax;
            productFromDataBase.Site = product.Site;
            productFromDataBase.OrderAuto = OrderAutoValue.OrderAuto;
            productFromDataBase.DayOrderAuto = OrderAutoValue.DayOrderAuto;
            productFromDataBase.Supplier = supplier;

            _context.Entry(productFromDataBase).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok();
        }

        #endregion

        #region HTTP PATCH

        [HttpPatch("{productId}/suppliers/{supplierId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(EmptyResult), Description = "La mise à jour partiel du produit a été un succès")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "Le fichier Json est incorrect")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "La modification dépasse le nombre de caractère autorisé")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID du produit ou du fournisseur renseigné est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteProductViewModels> PatchProduct(int productId, int supplierId, JsonPatchDocument<WriteProductViewModels> patchDoc)
        {
            //On vérifie que le supplier existe
            var supplier = _context.Supplier.Find(supplierId);
            if (supplier == null)
            {
                return NotFound();
            }

            //Récupération du produit en fonction de l'ID
            var productFromDataBase = _context.Product.Single(p => p.Id == productId);
            if (productFromDataBase == null)
            {
                return NotFound();
            }

            //Creation du produit temporaire avec les infos déjà existante
            var productToPatch = new WriteProductViewModels()
            {
                Name = productFromDataBase.Name,
                BarCode = productFromDataBase.BarCode,
                Price = productFromDataBase.Price,
                Description = productFromDataBase.Description,
                ImagePathFile = productFromDataBase.ImagePathFile,
                State = productFromDataBase.State,
                Quantity = productFromDataBase.Quantity,
                QuantityMax = productFromDataBase.QuantityMax,
                Site = productFromDataBase.Site,
                OrderAuto = productFromDataBase.OrderAuto,
                DayOrderAuto = productFromDataBase.DayOrderAuto,
                SupplierId = productFromDataBase.Supplier.Id

            };

            //Application des modifications dans le produit temporaire avec vérification du ModelState
            patchDoc.ApplyTo(productToPatch, ModelState);

            //Si le modèle n'est pas respecté avec ERREUR
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            //Vérifie que le ModelState est correcte pour les valeur du produit Temporaire
            if (!TryValidateModel(productToPatch))
            {
                return BadRequest(ModelState);
            }

            //Application du résultat dans la BDD
            productFromDataBase.Name = productToPatch.Name;
            productFromDataBase.BarCode = productToPatch.BarCode;
            productFromDataBase.Price = productToPatch.Price;
            productFromDataBase.Description = productToPatch.Description;
            productFromDataBase.ImagePathFile = productToPatch.ImagePathFile;
            productFromDataBase.State = productToPatch.State;
            productFromDataBase.Quantity = productToPatch.Quantity;
            productFromDataBase.QuantityMax = productToPatch.QuantityMax;
            productFromDataBase.Site = productToPatch.Site;
            productFromDataBase.OrderAuto = productFromDataBase.OrderAuto;
            productFromDataBase.DayOrderAuto = productToPatch.DayOrderAuto;
            productFromDataBase.Supplier.Id = productToPatch.SupplierId;

            _context.SaveChanges();

            return Ok();

        }

        [HttpPatch("OrderAuto")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(WriteProductViewModels), Description = "Le mode commande automatique a été activé/désactivé")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "Le fichier Json est incorrect")]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(EmptyResult), Description = "La table produit est vide")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteProductViewModels> PatchProductOrderAuto(JsonPatchDocument<WriteProductViewModels> patchDoc)
        {
            //Récupération du produit en fonction de l'ID
            var productFromDataBase = _context.Product.ToList();
            if (productFromDataBase == null)
            {
                return NoContent();
            }

            //Creation du produit temporaire avec les infos déjà existante
            var productToPatch = new WriteProductViewModels()
            {
                OrderAuto = false
            };

            //Application des modifications dans le produit temporaire avec vérification du ModelState
            patchDoc.ApplyTo(productToPatch, ModelState);

            //Si le modèle n'est pas respecté avec ERREUR
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            //Vérifie que le ModelState est correcte pour les valeur du produit Temporaire
            if (!TryValidateModel(productToPatch))
            {
                return BadRequest(ModelState);
            }

            //Application du résultat dans la BDD
            foreach (var lineproductFromDataBase in productFromDataBase)
            {
                lineproductFromDataBase.OrderAuto = productToPatch.OrderAuto;
            }

            _context.SaveChanges();

            return Ok();

        }

        #endregion

        #region HTTP DELETE

        [HttpDelete("{productId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(EmptyResult), Description = "La suppression du produit a été effectué avec succès")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID du produit est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(EmptyResult), Description = "Présence de clé étangère, impossible de supprimer le produit")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteProductViewModels> DeleteProduct(int productId)
        {
            //Récupération du produit en fonction de l'ID
            var productFromDataBase = _context.Product.Single(s => s.Id == productId);
            if (productFromDataBase == null)
            {
                return NotFound();
            }

            try
            {
                //Si il existe alors on peut le supprimer
                _context.Product.Remove(productFromDataBase);
                _context.SaveChanges();

                return Ok(); ;
            }
            catch (DbUpdateException) //Capture de l'erreur en cas de clé étrangère
            {
                return Conflict();

            }

            
        }

        #endregion
    }
}