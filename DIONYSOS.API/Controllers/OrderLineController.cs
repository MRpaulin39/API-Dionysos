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
    [Route("api/orderline")]
    [Produces("application/json")]
    public class OrderLineController : ControllerBase
    {

        //Ajout du contructeur puis injection du context pour la connexion à la BDD
        private readonly DionysosContext _context;

        public OrderLineController(DionysosContext context)
        {
            _context = context;
        }


        #region HTTP GET

        //Récupération de toutes les lignes de commandes clients
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ReadOrderLineViewModels), Description = "La récupération de toutes les lignes de commandes avec le produit et l'ID de l'entête de commande associé a été un succès")]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(EmptyResult), Description = "La table lignes de commande est vide")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<ReadOrderLineViewModels>> GetOrderLines()
        {
            //Si la table est vide
            if (!_context.OrderLine.Any())
            {
                return NoContent();
            }

            var OrderLineProductLink = _context.OrderLine
                .Join(
                    _context.Product,
                    OrderLine => OrderLine.Product.Id,
                    Product => Product.Id,
                    (OrderLine, Product) => new ReadOrderLineViewModels()
                    {
                        Id = OrderLine.Id,
                        Quantity = OrderLine.Quantity,
                        QuantityServed = OrderLine.QuantityServed,
                        ProductId = OrderLine.Product.Id,
                        ProductName = Product.Name,
                        ProductPrice = Product.Price,
                        OrderHeaderId = OrderLine.OrderHeader.Id

                    }
                ).ToList();

            return Ok(OrderLineProductLink);
        }

        //Récupération d'une ligne de commande en fonction de l'ID
        [HttpGet("{idOrderLine}", Name = "GetOrderLine")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ReadOrderLineViewModels), Description = "La récupération de la ligne de commande avec le produit et l'ID de l'entête de commande associé a été un succès")]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(EmptyResult), Description = "La table lignes de commande est vide")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID de la ligne de commande n'est pas connu de la base de données")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ReadOrderLineViewModels> GetOrderLineById(int idOrderLine)
        {
            //Si la table est vide
            if (!_context.OrderLine.Any())
            {
                return NoContent();
            }

            //Vérification si la ligne de commande avec l'id renseigné existe
            var orderLineToFind = _context.OrderLine.Find(idOrderLine);
            if (orderLineToFind == null)
            {
                return NotFound();
            }

            var OrderLineProductLink = _context.OrderLine
                .Where(o => o.Id == idOrderLine)
                .Join(
                    _context.Product,
                    OrderLine => OrderLine.Product.Id,
                    Product => Product.Id,
                    (OrderLine, Product) => new ReadOrderLineViewModels()
                    {
                        Id = OrderLine.Id,
                        Quantity = OrderLine.Quantity,
                        QuantityServed = OrderLine.QuantityServed,
                        ProductId = OrderLine.Product.Id,                        
                        ProductName = Product.Name,
                        ProductPrice = Product.Price,
                        OrderHeaderId = OrderLine.OrderHeader.Id,

                    }
                ).Single();

            return Ok(OrderLineProductLink);
        }

        //Récupération d'une ligne de commande en fonction de l'ID
        [HttpGet("orderheader/{idOrderHeader}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ReadOrderLineViewModels), Description = "La récupération de toutes les lignes de commandes avec le produit lié à l'ID de l'entête de commande associé réussi")]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(EmptyResult), Description = "La table lignes de commande est vide")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID de l'entête de commande est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ReadOrderLineViewModels> GetOrderLineByIdOrderHeader(int idOrderHeader)
        {
            //Si la table est vide
            if (!_context.OrderLine.Any())
            {
                return NoContent();
            }

            //Vérification si l'entete de commande avec l'id renseigné existe
            var orderHeaderToFind = _context.OrderHeader.Find(idOrderHeader);
            if (orderHeaderToFind == null)
            {
                return NotFound();
            }

            var OrderLineProductLink = _context.OrderLine
                .Where(o => o.OrderHeader.Id == idOrderHeader)
                .Join(
                    _context.Product,
                    OrderLine => OrderLine.Product.Id,
                    Product => Product.Id,
                    (OrderLine, Product) => new ReadOrderLineViewModels()
                    {
                        Id = OrderLine.Id,
                        Quantity = OrderLine.Quantity,
                        QuantityServed = OrderLine.QuantityServed,
                        ProductId = OrderLine.Product.Id,
                        ProductName = Product.Name,
                        ProductPrice = Product.Price,
                        OrderHeaderId = OrderLine.OrderHeader.Id,

                    }
                ).ToList();

            return Ok(OrderLineProductLink);
        }

        #endregion

        #region HTTP POST
        //Permet de rajouter une ligne de commande à l'entête de commande
        [HttpPost("product/{productId}/orderheader/{orderHeaderId}")]
        [SwaggerResponse(HttpStatusCode.Created, typeof(WriteOrderLineViewModels), Description = "La création de la ligne de commande a été un succès")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(WriteOrderSupplierViewModels), Description = "Le fichier Json est incorrect")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "Une ou plusieurs valeurs dépassent le nombre de caractère autorisé")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID du produit ou de l'entête de commande renseigné est incorrect")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteOrderLineViewModels> CreateOrderLine(int productId, int orderHeaderId, WriteOrderLineViewModels orderLine)
        {
            //On vérifie que le JSON reçu est conforme au WriteModels
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            OrderLine finalOrderLine = new OrderLine();

            var product = _context.Product.Find(productId);
            if (product == null)
            {
                return NotFound();
            }

            var orderheader = _context.OrderHeader.Find(orderHeaderId);
            if (orderheader == null)
            {
                return NotFound();
            }

            finalOrderLine.Product = product;
            finalOrderLine.OrderHeader = orderheader;
            finalOrderLine.Quantity = orderLine.Quantity;
            finalOrderLine.QuantityServed = orderLine.QuantityServed;



            _context.OrderLine.Add(finalOrderLine);
            _context.SaveChanges();

            return CreatedAtRoute("GetOrderLine", new { idOrderLine = finalOrderLine.Id }, finalOrderLine);
        }

        #endregion

        #region HTTP PUT
        [HttpPut("{orderLineId}/product/{productId}/orderheader/{orderheaderId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(WriteOrderLineViewModels), Description = "La mise à jour de la ligne de commande a été un succès")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(WriteOrderLineViewModels), Description = "Le fichier Json est incorrect")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "Une ou plusieurs valeurs dépassent le nombre de caractère autorisé")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID du produit, de l'entête de commande ou l'ID de la ligne de commande renseigné est incorrect")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteOrderLineViewModels> UpdateOrderLine(int orderLineId, int productId, int orderHeaderId, WriteOrderLineViewModels orderLine)
        {
            //On vérifie si le ModelState est respecté
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var OrderLineFromDataBase = _context.OrderLine.Single(o => o.Id == orderLineId);
            if (orderLine == null)
            {
                return NotFound();
            }

            var product = _context.Product.Find(productId);
            if (product == null)
            {
                return NotFound();
            }

            var orderheader = _context.OrderHeader.Find(orderHeaderId);
            if (orderheader == null)
            {
                return NotFound();
            }

            OrderLineFromDataBase.Product = product;
            OrderLineFromDataBase.OrderHeader = orderheader;
            OrderLineFromDataBase.Quantity = orderLine.Quantity;
            OrderLineFromDataBase.QuantityServed = orderLine.QuantityServed;

            _context.Entry(OrderLineFromDataBase).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok();
        }

        #endregion

        #region HTTP PATCH

        [HttpPatch("{orderLineId}/product/{productId}/orderheader/{orderheaderId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(EmptyResult), Description = "La mise à jour partiel de la ligne de commande a été un succès")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "Le fichier Json est incorrect")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "La modification dépasse le nombre de caractère autorisé")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID du produit, de l'entête de commande ou l'ID de la ligne de commande renseigné est incorrect")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteOrderLineViewModels> PatchOrderLine(int orderLineId, int productId, int orderHeaderId, JsonPatchDocument<WriteOrderLineViewModels> patchDoc)
        {
            //On vérifie que le produit existe
            var product = _context.Product.Find(productId);
            if (product == null)
            {
                return NotFound();
            }

            //On vérifie que l'entete de commande existe
            var orderheader = _context.OrderHeader.Find(orderHeaderId);
            if (orderheader == null)
            {
                return NotFound();
            }

            //Récupération de l'entete de commande en fonction de l'ID
            var OrderLineFromDataBase = _context.OrderLine.Single(o => o.Id == orderLineId);
            if (OrderLineFromDataBase == null)
            {
                return NotFound();
            }

            //Creation de la ligne de commande temporaire avec les infos déjà existante
            var PatchOrderLine = new WriteOrderLineViewModels()
            {
                Quantity = OrderLineFromDataBase.Quantity,
                QuantityServed = OrderLineFromDataBase.QuantityServed,
                ProductId = OrderLineFromDataBase.Product.Id,
                OrderHeaderId = OrderLineFromDataBase.OrderHeader.Id

            };

            //Application des modifications dans l'entete de commande temporaire avec vérification du ModelState
            patchDoc.ApplyTo(PatchOrderLine, ModelState);

            //Si le modèle n'est pas respecté avec ERREUR
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            //Vérifie que le ModelState est correcte pour les valeur de la ligne de commande Temporaire
            if (!TryValidateModel(PatchOrderLine))
            {
                return BadRequest(ModelState);
            }

            //Application du résultat dans la BDD
            OrderLineFromDataBase.Quantity = PatchOrderLine.Quantity;
            OrderLineFromDataBase.QuantityServed = PatchOrderLine.QuantityServed;
            OrderLineFromDataBase.Product.Id = PatchOrderLine.ProductId;
            OrderLineFromDataBase.OrderHeader.Id = PatchOrderLine.OrderHeaderId;

            _context.SaveChanges();

            return Ok();

        }

        #endregion

        #region HTTP DELETE

        [HttpDelete("{orderLineId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(EmptyResult), Description = "La suppression de la ligne de commande a été effectué avec succès")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID de la ligne de commande est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(EmptyResult), Description = "Présence de clé étangère, impossible de supprimer la ligne de commande")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteOrderLineViewModels> DeleteOrderLine(int orderLineId)
        {
            //Récupération de la ligne de commande en fonction de l'ID
            var OrderLineFromDataBase = _context.OrderLine.Single(o => o.Id == orderLineId);
            if (OrderLineFromDataBase == null)
            {
                return NotFound();
            }

            try
            {
                //Si la ligne de commande existe, on peut la supprimer
                _context.OrderLine.Remove(OrderLineFromDataBase);
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
