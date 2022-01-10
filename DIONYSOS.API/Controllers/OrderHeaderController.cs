using DIONYSOS.API.Context;
using DIONYSOS.API.Models;
using DIONYSOS.API.ViewModels;
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
    [Route("api/orderheader")]
    [Produces("application/json")]
    public class OrderHeaderController : ControllerBase
    {

        //Ajout du contructeur puis injection du context pour la connexion à la BDD
        private readonly DionysosContext _context;

        public OrderHeaderController(DionysosContext context)
        {
            _context = context;
        }


        #region HTTP GET

        //Récupération de tout les entêtes de commandes
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ReadOrderHeaderViewModels), Description = "La récupération des entêtes de commande avec le client associé a été un succès")]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(EmptyResult), Description = "La table entête de commande est vide")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<ReadOrderHeaderViewModels>> GetOrderHearders()
        {
            //Si la table est vide
            if (!_context.OrderHeader.Any())
            {
                return NoContent();

            }

            var OrderHeaderSupplierLink = _context.OrderHeader
                .Join(
                    _context.Customers,
                    OrderHeader => OrderHeader.Customer.Id,
                    Customer => Customer.Id,
                    (OrderHeader, Customer) => new
                    {
                        Id = OrderHeader.Id,
                        Paid = OrderHeader.Paid,
                        DateOrder = OrderHeader.DateOrder,
                        CustomerId = OrderHeader.Customer.Id,
                        CustomerName = Customer.Name,

                    }
                ).ToList();

            return Ok(OrderHeaderSupplierLink);
        }

        //Récupération d'une commande fournisseur en fonction de l'ID du Head Order
        [HttpGet("{idOrderHeader}", Name = "GetOrderHeader")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ReadOrderHeaderViewModels), Description = "La récupération de l'entête de commande avec le client associé a été un succès")]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(EmptyResult), Description = "La table entête de commande est vide")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID de l'entête de commande est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ReadOrderHeaderViewModels> GetOrderHeaderById(int idOrderHeader)
        {
            //Si la table est vide
            if (!_context.OrderHeader.Any())
            {
                return NoContent();

            }

            //Vérification si l'entete de commande avec l'id renseigné existe
            var orderHeaderToFind = _context.OrderHeader.Find(idOrderHeader);
            if (orderHeaderToFind == null)
            {
                return NotFound();
            }

            var OrderHeaderSupplierLink = _context.OrderHeader
                .Where(o => o.Id == idOrderHeader)
                .Join(
                    _context.Customers,
                    OrderHeader => OrderHeader.Customer.Id,
                    Customer => Customer.Id,
                    (OrderHeader, Customer) => new ReadOrderHeaderViewModels()
                    {
                        Id = OrderHeader.Id,
                        Paid = OrderHeader.Paid,
                        DateOrder = OrderHeader.DateOrder,
                        CustomerId = OrderHeader.Customer.Id,
                        CustomerName = Customer.Name

                    }
                ).Single();

            return Ok(OrderHeaderSupplierLink);
        }

        #endregion

        #region HTTP POST

        //Permet de rajouter une commande au client
        [HttpPost("customer/{customerId}")]
        [SwaggerResponse(HttpStatusCode.Created, typeof(WriteOrderHeaderViewModels), Description = "La création de l'entête de commande a été effectué avec succès")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(WriteOrderSupplierViewModels), Description = "Le fichier Json est incorrect")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "Une ou plusieurs valeurs dépassent le nombre de caractère autorisé")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID du client renseigné est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteOrderHeaderViewModels> CreateOrderHeader(int customerId, WriteOrderHeaderViewModels orderHeader)
        {
            //On vérifie que le JSON reçu est conforme au WriteModels
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            OrderHeader finalOrderHeader = new OrderHeader();

            var customer = _context.Customers.Find(customerId);
            if (customer == null)
            {
                return NotFound();
            }

            finalOrderHeader.Customer = customer;
            finalOrderHeader.Paid = orderHeader.Paid;
            finalOrderHeader.DateOrder = orderHeader.DateOrder;


            _context.OrderHeader.Add(finalOrderHeader);
            _context.SaveChanges();

            return CreatedAtRoute("GetOrderHeader", new { idOrderHeader = finalOrderHeader.Id }, finalOrderHeader);
        }

        #endregion

        #region HTTP PUT

        [HttpPut("{orderHeaderId}/customer/{customerId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(WriteOrderHeaderViewModels), Description = "La mise à jour de l'entête de commande a été effectué avec succès")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(WriteOrderHeaderViewModels), Description = "Le fichier Json est incorrect")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "Une ou plusieurs valeurs dépassent le nombre de caractère autorisé")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID de l'entete de commande ou du client renseigné est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteOrderHeaderViewModels> UpdateOrderHeader(int orderHeaderId, int customerId, WriteOrderHeaderViewModels orderHeader)
        {
            //On vérifie que le JSON reçu est conforme au ViewModels
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var OrderHeaderFromDataBase = _context.OrderHeader.Single(o => o.Id == orderHeaderId);
            if (orderHeader == null)
            {
                return NotFound();
            }

            var customer = _context.Customers.Find(customerId);
            if (customer == null)
            {
                return NotFound();
            }

            OrderHeaderFromDataBase.Customer = customer;
            OrderHeaderFromDataBase.Paid = orderHeader.Paid;
            OrderHeaderFromDataBase.DateOrder = orderHeader.DateOrder;

            _context.Entry(OrderHeaderFromDataBase).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok();
        }

        #endregion

        #region HTTP PATCH

        [HttpPatch("{orderHeaderId}/customer/{customerId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(EmptyResult), Description = "La mise à jour partiel de l'entête a été effectué avec succès")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "Le fichier Json est incorrect")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "La modification dépasse le nombre de caractère autorisé")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID de l'entête de commande ou du client renseigné est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteOrderHeaderViewModels> PatchOrderHeader(int orderHeaderId, int customerId, JsonPatchDocument<WriteOrderHeaderViewModels> patchDoc)
        {
            //On vérifie que le client existe
            var customer = _context.Customers.Find(customerId);
            if (customer == null)
            {
                return NotFound();
            }

            //Récupération de l'entete de commande en fonction de l'ID
            var OrderHeaderFromDataBase = _context.OrderHeader.Single(o => o.Id == orderHeaderId);
            if (OrderHeaderFromDataBase == null)
            {
                return NotFound();
            }

            //Creation de l'entete de commande temporaire avec les infos déjà existante
            var PatchOrderHeader = new WriteOrderHeaderViewModels()
            {
                Paid = OrderHeaderFromDataBase.Paid,
                DateOrder = OrderHeaderFromDataBase.DateOrder,
                CustomerId = OrderHeaderFromDataBase.Customer.Id

            };

            //Application des modifications dans l'entete de commande temporaire avec vérification du ModelState
            patchDoc.ApplyTo(PatchOrderHeader, ModelState);

            //Si le modèle n'est pas respecté avec ERREUR
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            //Vérifie que le ModelState est correcte pour les valeur de l'entete de commande Temporaire
            if (!TryValidateModel(PatchOrderHeader))
            {
                return BadRequest(ModelState);
            }

            //Application du résultat dans la BDD
            OrderHeaderFromDataBase.Paid = PatchOrderHeader.Paid;
            OrderHeaderFromDataBase.DateOrder = PatchOrderHeader.DateOrder;
            OrderHeaderFromDataBase.Customer.Id = PatchOrderHeader.CustomerId;

            _context.SaveChanges();

            return Ok();

        }

        #endregion

        #region HTTP DELETE

        [HttpDelete("{orderHeaderId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(EmptyResult), Description = "La suppression de l'entête de commande a été effectué avec succès")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID de l'entête de commande est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(EmptyResult), Description = "Présence de clé étangère, impossible de supprimer l'entête de commande")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteOrderHeaderViewModels> DeleteOrderHeader(int orderHeaderId)
        {
            //Récupération de l'entete de commande en fonction de son ID
            var OrderHeaderFromDataBase = _context.OrderHeader.Single(o => o.Id == orderHeaderId);
            if (OrderHeaderFromDataBase == null)
            {
                return NotFound();
            }

            try
            {
                _context.OrderHeader.Remove(OrderHeaderFromDataBase);
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
