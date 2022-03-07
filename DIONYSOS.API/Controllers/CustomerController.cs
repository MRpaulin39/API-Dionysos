using DIONYSOS.API.Context;
using DIONYSOS.API.Data.Models;
using DIONYSOS.API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace DIONYSOS.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Administrator")]
    [Route("api/customers")]
    [Produces("application/json")]
    [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(EmptyResult), Description = "Veuillez vous authentifier à l'API")]
    [SwaggerResponse(HttpStatusCode.Forbidden, typeof(EmptyResult), Description = "Vous n'avez pas les privillèges nécessaires")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class CustomerController : ControllerBase
    {
        //Ajout du contructeur puis injection du context pour la connexion à la BDD
        private readonly DionysosContext _context;

        public CustomerController(DionysosContext context)
        {
            _context = context;
        }


        #region HTTP GET

        //Récupération de tout les clients
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ReadCustomerViewModels), Description = "La récupération de tous les clients a été un succès")]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(EmptyResult), Description = "La table client est vide")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<ReadCustomerViewModels>> GetCustomers()
        {
            //Si la table est vide
            if (!_context.Customers.Any())
            {
                return NoContent();
            }

            List<ReadCustomerViewModels> customersVM = new List<ReadCustomerViewModels>();

            var customers = _context.Customers
                            .OrderBy(s => s.Name)
                            .ToList();

            foreach (Customer s in customers)
            {
                ReadCustomerViewModels customerVM = new ReadCustomerViewModels();
                customerVM.Id = s.Id;
                customerVM.Name = s.Name;
                customerVM.Gender = s.Gender;
                customerVM.FirstName = s.FirstName;
                customerVM.Adress = s.Adress;
                customerVM.FidCard = s.FidCard;
                customerVM.Phone = s.Phone;
                customerVM.Mail = s.Mail;
                customerVM.City = s.City;
                customerVM.ZipCode = s.ZipCode;
                customerVM.Password = s.Password;
                customersVM.Add(customerVM);
            }

            return Ok(customers);
        }

        //Récupération d'un client en fonction de l'ID
        [HttpGet("{idCustomer}", Name = "GetCustomer")]
        [Authorize(Roles = "Administrator,AuthUser")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ReadCustomerViewModels), Description = "La récupération de du client via son ID a été un succès")]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(EmptyResult), Description = "La table client est vide")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID client renseigné est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ReadCustomerViewModels> GetSupplierById(int idCustomer)
        {
            //Si la table est vide
            if (!_context.Customers.Any())
            {
                return NoContent();

            }

            ReadCustomerViewModels customerVM = new ReadCustomerViewModels();

            var customerToReturn = _context.Customers
                                    .Where(s => s.Id == idCustomer)
                                    .Single();

            if (customerToReturn == null)
            {
                return NotFound();
            }

            customerVM.Id = customerToReturn.Id;
            customerVM.Name = customerToReturn.Name;
            customerVM.Gender = customerToReturn.Gender;
            customerVM.FirstName = customerToReturn.FirstName;
            customerVM.Adress = customerToReturn.Adress;
            customerVM.FidCard = customerToReturn.FidCard;
            customerVM.Phone = customerToReturn.Phone;
            customerVM.Mail = customerToReturn.Mail;
            customerVM.City = customerToReturn.City;
            customerVM.ZipCode = customerToReturn.ZipCode;
            customerVM.Password = customerToReturn.Password;

            return Ok(customerVM);
        }

        
        //Récupération d'un client en fonction de l'ID
        [HttpGet("mail/{mailCustomer}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ReadCustomerViewModels), Description = "La récupération du client via son email a été un succès")]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(EmptyResult), Description = "La table client est vide")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'email duz client renseigné est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ReadCustomerViewModels> GetSupplierByMail(string mailCustomer)
        {
            //Si la table est vide
            if (!_context.Customers.Any())
            {
                return NoContent();

            }

            ReadCustomerViewModels customerVM = new ReadCustomerViewModels();

            var customerToReturn = _context.Customers
                                    .Where(s => s.Mail == mailCustomer)
                                    .FirstOrDefault();

            if (customerToReturn == null)
            {
                return NotFound();
            }

            customerVM.Id = customerToReturn.Id;
            customerVM.Name = customerToReturn.Name;
            customerVM.Gender = customerToReturn.Gender;
            customerVM.FirstName = customerToReturn.FirstName;
            customerVM.Adress = customerToReturn.Adress;
            customerVM.FidCard = customerToReturn.FidCard;
            customerVM.Phone = customerToReturn.Phone;
            customerVM.Mail = customerToReturn.Mail;
            customerVM.City = customerToReturn.City;
            customerVM.ZipCode = customerToReturn.ZipCode;
            customerVM.Password = customerToReturn.Password;

            return Ok(customerVM);
        }

        #endregion

        //ToDo : Ajouter une vérification sur les clients afin de vérifier qu'il n'exsite pas encore

        #region HTTP POST

        //Permet de rajouter un client
        [HttpPost]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.Created, typeof(WriteCustomerViewModels), Description = "La création du client a été effectué avec succès")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(WriteOrderSupplierViewModels), Description = "Le fichier Json est incorrect")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "Une ou plusieurs valeurs dépassent le nombre de caractère autorisé")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteCustomerViewModels> CreateCustomer(WriteCustomerViewModels customer)
        {
            //On vérifie que le JSON reçu est conforme au WriteModels
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            Customer finalCustomer = new Customer();

            finalCustomer.Name = customer.Name;
            finalCustomer.Gender = customer.Gender;
            finalCustomer.FirstName = customer.FirstName;
            finalCustomer.Adress = customer.Adress;
            finalCustomer.FidCard = customer.FidCard;
            finalCustomer.Phone = customer.Phone;
            finalCustomer.Mail = customer.Mail;
            finalCustomer.City = customer.City;
            finalCustomer.ZipCode = customer.ZipCode;
            finalCustomer.Password = customer.Password;

  
            _context.Customers.Add(finalCustomer);
            _context.SaveChanges();

            return CreatedAtRoute("GetCustomer", new { idCustomer = finalCustomer.Id }, finalCustomer);

        }

        #endregion

        #region HTTP PUT

        [HttpPut("{customerId}")]
        [Authorize(Roles = "Administrator,AuthUser")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(WriteCustomerViewModels), Description = "La mise à jour des informations clients réalisé avec succès")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(WriteCustomerViewModels), Description = "Le fichier Json est incorrect")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "Une ou plusieurs valeurs dépassent le nombre de caractère autorisé")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID du client est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteCustomerViewModels> UpdateCustomer(int customerId, WriteCustomerViewModels customer)
        {
            //On vérifie que le JSON reçu est conforme au ViewModels
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customerFromDataBase = _context.Customers.Single(s => s.Id == customerId);
            if (customer == null)
            {
                return NotFound();
            }

            customerFromDataBase.Name = customer.Name;
            customerFromDataBase.Gender = customer.Gender;
            customerFromDataBase.FirstName = customer.FirstName;
            customerFromDataBase.Adress = customer.Adress;
            customerFromDataBase.FidCard = customer.FidCard;
            customerFromDataBase.Phone = customer.Phone;
            customerFromDataBase.Mail = customer.Mail;
            customerFromDataBase.City = customer.City;
            customerFromDataBase.ZipCode = customer.ZipCode;
            customerFromDataBase.Password = customer.Password;

            _context.Entry(customerFromDataBase).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok();
        }

        #endregion

        #region HTTP PATCH
        [HttpPatch("{customerId}")]
        [Authorize(Roles = "Administrator,AuthUser")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(EmptyResult), Description = "La mise à jour partiel du client a été effectué avec succès")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "Le fichier Json est incorrect")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "La modification dépasse le nombre de caractère autorisé")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID du client renseigné est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteCustomerViewModels> PatchCustumer(int customerId, JsonPatchDocument<WriteCustomerViewModels> patchDoc)
        {
            //Récupération du fournisseur en fonction de l'ID
            var customerFromDataBase = _context.Customers.Single(s => s.Id == customerId);
            if (customerFromDataBase == null)
            {
                return NotFound();
            }

            //Creation du fournisseur temporaire avec les infos déjà existante
            var customerToPatch = new WriteCustomerViewModels()
            {
                Name = customerFromDataBase.Name,
                Gender = customerFromDataBase.Gender,
                FirstName = customerFromDataBase.FirstName,
                Adress = customerFromDataBase.Adress,
                FidCard = customerFromDataBase.FidCard,
                Phone = customerFromDataBase.Phone,
                Mail = customerFromDataBase.Mail,
                City = customerFromDataBase.City,
                ZipCode = customerFromDataBase.ZipCode,
                Password = customerFromDataBase.Password

            };

            //Application des modifications dans le client temporaire avec vérification du ModelState
            patchDoc.ApplyTo(customerToPatch, ModelState);

            //Si le modèle n'est pas respecté avec ERREUR
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            //Vérifie que le ModelState est correcte pour les valeur Goal Temporaire
            if (!TryValidateModel(customerToPatch))
            {
                return BadRequest(ModelState);
            }

            //Application du résultat dans la BDD
            customerFromDataBase.Name = customerToPatch.Name;
            customerFromDataBase.Gender = customerToPatch.Gender;
            customerFromDataBase.FirstName = customerToPatch.FirstName;
            customerFromDataBase.Adress = customerToPatch.Adress;
            customerFromDataBase.FidCard = customerToPatch.FidCard;
            customerFromDataBase.Phone = customerToPatch.Phone;
            customerFromDataBase.Mail = customerToPatch.Mail;
            customerFromDataBase.City = customerToPatch.City;
            customerFromDataBase.ZipCode = customerToPatch.ZipCode;
            customerFromDataBase.Password = customerToPatch.Password;

            _context.SaveChanges();

            return Ok();

        }

        #endregion

        #region HTTP DELETE

        [HttpDelete("{customerId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(EmptyResult), Description = "La suppression du client a été effectué avec succès")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID du client est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(EmptyResult), Description = "Présence de clé étangère, impossible de supprimer le client")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteCustomerViewModels> DeleteCustomer(int customerId)
        {
            //Récupération du client en fonction de l'ID
            var customerFromDataBase = _context.Customers.Single(s => s.Id == customerId);
            if (customerFromDataBase == null)
            {
                return NotFound();
            }

            try
            {
                //Si le client existe alors on peu le supprimer
                _context.Customers.Remove(customerFromDataBase);
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
