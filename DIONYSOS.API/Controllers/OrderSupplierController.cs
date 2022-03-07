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
    [Authorize]
    [Route("api/ordersupplier")]
    [Produces("application/json")]
    public class OrderSupplierController : ControllerBase
    {
        //Ajout du contructeur puis injection du context pour la connexion à la BDD
        private readonly DionysosContext _context;

        public OrderSupplierController(DionysosContext context)
        {
            _context = context;
        }


        #region HTTP GET

        //Récupération de toutes les commandes fournisseurs
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ReadOrderSupplierViewModels), Description = "La récupération de toutes les commandes fournisseurs a été un succès")]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(EmptyResult), Description = "La table de commandes fournisseur est vide")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<ReadOrderSupplierViewModels>> GetOrderSuppliers()
        {
            //Si la table est vide
            if (!_context.Supplier.Any())
            {
                return NoContent();
            }

            var OrderSupplierSupplierLink = _context.OrderSupplier
                .Join(
                    _context.Product,
                    OrderSupplier => OrderSupplier.Product.Id,
                    Product => Product.Id,
                    (OrderSupplier, Product) => new ReadOrderSupplierViewModels()
                    {
                        Id = OrderSupplier.Id,
                        Quantity = OrderSupplier.Quantity,
                        DateOrder = OrderSupplier.DateOrder,
                        Receive = OrderSupplier.Receive,
                        ProductId = Product.Id,
                        ProductName = Product.Name,

                    }
                ).ToList();

            return Ok(OrderSupplierSupplierLink);
        }

        //Récupération du nombre de commandes fournisseurs non reçu en fonction de l'id du produit
        [HttpGet("unreceived/{productId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ReadOrderSupplierViewModels), Description = "La récupération du nombre de commande fournisseur en fonction de l'id produit a été un succès")]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(EmptyResult), Description = "La table de commandes fournisseur est vide")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID du produit renseigné est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<ReadOrderSupplierViewModels>> GetOrderSuppliersUnreceive(int productId)
        {
            //Si la table est vide
            if (!_context.OrderSupplier.Any())
            {
                return NoContent();

            }

            //Vérification si le produit avec l'id renseigné existe
            var productToFind = _context.Product.Find(productId);
            if (productToFind == null)
            {
                return NotFound();
            }

            var OrderSupplierCount = _context.OrderSupplier
                .Where(o => o.Receive == false && o.Product.Id == productId)
                .Count();

            return Ok(OrderSupplierCount);
        }

        //Récupération d'une commande fournisseur en fonction de l'ID
        [HttpGet("{idOrderSupplier}", Name = "GetOrderSupplier")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ReadOrderSupplierViewModels), Description = "La récupération de la commande fournisseur a été un succès")]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(EmptyResult), Description = "La table de commandes fournisseur est vide")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'id de l'OrderSupplier renseigné est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ReadOrderSupplierViewModels> GetOrderSupplierById(int idOrderSupplier)
        {
            //Si la table est vide
            if (!_context.OrderSupplier.Any())
            {
                return NoContent();

            }

            //Vérification si l'OrderSupplier avec l'id renseigné existe
            var orderSupplierToFind = _context.OrderSupplier.Find(idOrderSupplier);
            if (orderSupplierToFind == null)
            {
                return NotFound();
            }

            var orderSupplierSupplierLink = _context.OrderSupplier
                .Where(o => o.Id == idOrderSupplier)
                .Join(
                    _context.Product,
                    OrderSupplier => OrderSupplier.Product.Id,
                    Product => Product.Id,
                    (OrderSupplier, Product) => new ReadOrderSupplierViewModels()
                    {
                        Id = OrderSupplier.Id,
                        Quantity = OrderSupplier.Quantity,
                        DateOrder = OrderSupplier.DateOrder,
                        Receive = OrderSupplier.Receive,
                        ProductId = Product.Id,
                        ProductName = Product.Name,

                    }
                ).Single();

            return Ok(orderSupplierSupplierLink);
        }

        #endregion

        #region HTTP POST

        //Permet de rajouter une commande au fournisseur
        [HttpPost("product/{productId}")]
        [SwaggerResponse(HttpStatusCode.Created, typeof(WriteOrderSupplierViewModels), Description = "La création de la commande fournisseur a été effectué avec succès")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(WriteSupplierViewModels), Description = "Le fichier Json est incorrect")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "Une ou plusieurs valeurs dépassent le nombre de caractère autorisé")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID du produit renseigné est incorrect")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteOrderSupplierViewModels> CreateOrderSupplier(int productId, WriteOrderSupplierViewModels orderSupplier)
        {
            //On vérifie que le JSON reçu est conforme au WriteModels
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            OrderSupplier finalOrderSupplier = new OrderSupplier();

            var product = _context.Product.Find(productId);
            if (product == null)
            {
                return NotFound();
            }

            finalOrderSupplier.Product = product;
            finalOrderSupplier.Quantity = orderSupplier.Quantity;
            finalOrderSupplier.DateOrder = orderSupplier.DateOrder;
            finalOrderSupplier.Receive = false;

            _context.OrderSupplier.Add(finalOrderSupplier);
            _context.SaveChanges();

            return CreatedAtRoute("GetOrderSupplier", new { idOrderSupplier = finalOrderSupplier.Id }, finalOrderSupplier);
        }

        #endregion

        #region HTTP PUT

        [HttpPut("{orderSupplierId}/product/{productId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(WriteOrderSupplierViewModels), Description = "La mise à jour de la commande fournisseur a été effectué avec succès")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(WriteOrderSupplierViewModels), Description = "Le fichier Json est incorrect")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "Une ou plusieurs valeurs dépassent le nombre de caractère autorisé")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID de la commande fournisseur ou du produit renseigné est incorrect")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteOrderSupplierViewModels> UpdateOrderSupplier(int orderSupplierId, int productId, WriteOrderSupplierViewModels orderSupplier)
        {
            //On vérifie que le JSON reçu est conforme au ViewModels
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var OrderSupplierFromDataBase = _context.OrderSupplier.Single(o => o.Id == orderSupplierId);
            if (orderSupplier == null)
            {
                return NotFound();
            }

            var product = _context.Product.Find(productId);
            if (product == null)
            {
                return NotFound();
            }

            OrderSupplierFromDataBase.Product = product;
            OrderSupplierFromDataBase.Quantity = orderSupplier.Quantity;
            OrderSupplierFromDataBase.DateOrder = orderSupplier.DateOrder;
            OrderSupplierFromDataBase.Receive = orderSupplier.Receive;

            _context.Entry(OrderSupplierFromDataBase).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok();
        }

        #endregion

        #region HTTP PATCH

        [HttpPatch("{orderSupplierId}/product/{productId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(EmptyResult), Description = "La mise à jour partiel de la commande fournisseur a été effectué avec succès")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "Le fichier Json est incorrect")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "La modification dépasse le nombre de caractère autorisé")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID de la commande fournisseur ou du produit renseigné est incorrect")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteOrderSupplierViewModels> PatchOrderSupplier(int orderSupplierId, int productId, JsonPatchDocument<WriteOrderSupplierViewModels> patchDoc)
        {
            //On vérifie que le produit existe
            var product = _context.Product.Find(productId);
            if (product == null)
            {
                return NotFound();
            }

            //Récupération du fournisseur en fonction de l'ID
            var OrderSupplierFromDataBase = _context.OrderSupplier.Single(o => o.Id == orderSupplierId);
            if (OrderSupplierFromDataBase == null)
            {
                return NotFound();
            }

            //Creation du fournisseur temporaire avec les infos déjà existante
            var orderSupplierToPatch = new WriteOrderSupplierViewModels()
            {

                Quantity = OrderSupplierFromDataBase.Quantity,
                DateOrder = OrderSupplierFromDataBase.DateOrder,
                Receive = OrderSupplierFromDataBase.Receive,
                ProductId = OrderSupplierFromDataBase.Product.Id

            };

            //Application des modifications dans le Goal temporaire avec vérification du ModelState
            patchDoc.ApplyTo(orderSupplierToPatch, ModelState);

            //Si le modèle n'est pas respecté avec ERREUR
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            //Vérifie que le ModelState est correcte pour les valeur Goal Temporaire
            if (!TryValidateModel(orderSupplierToPatch))
            {
                return BadRequest(ModelState);
            }

            //Application du résultat dans la BDD
            OrderSupplierFromDataBase.Quantity = orderSupplierToPatch.Quantity;
            OrderSupplierFromDataBase.DateOrder = orderSupplierToPatch.DateOrder;
            OrderSupplierFromDataBase.Receive = orderSupplierToPatch.Receive;
            OrderSupplierFromDataBase.Product.Id = orderSupplierToPatch.ProductId;

            _context.SaveChanges();

            return Ok();

        }

        #endregion

        #region HTTP DELETE

        [HttpDelete("{orderSupplierId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(EmptyResult), Description = "La suppression de la commande fournisseur a été effectuée avec succès")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID de la commande fournisseur est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(EmptyResult), Description = "Présence de clé étangère, impossible de supprimer la commande fournisseur")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteOrderSupplierViewModels> DeleteAlcohol(int orderSupplierId)
        {
            //Récupération de la commande fournisseur
            var OrderSupplierFromDataBase = _context.OrderSupplier.Single(o => o.Id == orderSupplierId);
            if (OrderSupplierFromDataBase == null)
            {
                return NotFound();
            }

            try
            {
                //Si elle existe alors on la supprime
                _context.OrderSupplier.Remove(OrderSupplierFromDataBase);
                _context.SaveChanges();

                return Ok();
            }
            catch (DbUpdateException) //Capture de l'erreur en cas de clé étrangère
            {
                return Conflict();
            }

            
        }

        #endregion
    }
}
