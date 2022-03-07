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
    [Route("api/suppliers")]
    [Produces("application/json")]
    [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(EmptyResult), Description = "Veuillez vous authentifier à l'API")]
    [SwaggerResponse(HttpStatusCode.Forbidden, typeof(EmptyResult), Description = "Vous n'avez pas les privillèges nécessaires")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class SuppliersController : ControllerBase
    {
        //Ajout du contructeur puis injection du context pour la connexion à la BDD
        private readonly DionysosContext _context;

        public SuppliersController(DionysosContext context)
        {
            _context = context;
        }


        #region HTTP GET

        //Récupération de tous les fournisseurs
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ReadSupplierViewModels), Description = "La récupération des fournisseurs a été un succès")]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(EmptyResult), Description = "La table fournisseur est vide")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<ReadSupplierViewModels>> GetSuppliers()
        {
            //Si la table est vide
            if (!_context.Supplier.Any())
            {
                return NoContent();
            }

            List<ReadSupplierViewModels> suppliersVM = new List<ReadSupplierViewModels>();

            var suppliers = _context.Supplier
                            .OrderBy(s => s.Name)
                            .ToList();

            foreach (Supplier s in suppliers)
            {
                ReadSupplierViewModels supplierVM = new ReadSupplierViewModels();
                supplierVM.Id = s.Id;
                supplierVM.Name = s.Name;
                supplierVM.Adress = s.Adress;
                supplierVM.City = s.City;
                supplierVM.ZipCode = s.ZipCode;
                supplierVM.Phone = s.Phone;
                supplierVM.Mail = s.Mail;
                suppliersVM.Add(supplierVM);
            }

            return Ok(suppliersVM);
        }

        //Récupération d'un fournisseur en fonction de l'ID
        [HttpGet("{idSupplier}", Name = "GetSupplier")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ReadSupplierViewModels), Description = "La récupération du fournisseur a été un succès")]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(EmptyResult), Description = "La table fournisseur est vide")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'id du fournisseur renseigné n'est pas connue de la base de données")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ReadSupplierViewModels> GetSupplierById(int idSupplier)
        {
            //Si la table est vide
            if (!_context.Supplier.Any())
            {
                return NoContent();

            }

            //Vérification si le produit avec l'id renseigné existe
            var supplierToFind = _context.Supplier.Find(idSupplier);
            if (supplierToFind == null)
            {
                return NotFound();
            }

            ReadSupplierViewModels supplierVM = new ReadSupplierViewModels();

            var supplierToReturn = _context.Supplier
                                    .Where(s => s.Id == idSupplier)
                                    .Single();

            if (supplierToReturn == null)
            {
                return NotFound();
            }

            supplierVM.Id = supplierToReturn.Id;
            supplierVM.Name = supplierToReturn.Name;
            supplierVM.Adress = supplierToReturn.Adress;
            supplierVM.City = supplierToReturn.City;
            supplierVM.ZipCode = supplierToReturn.ZipCode;
            supplierVM.Phone = supplierToReturn.Phone;
            supplierVM.Mail = supplierToReturn.Mail;

            return Ok(supplierVM);
        }

        #endregion

        //ToDo : Ajouter une vérification sur fournisseur pas déjà existant

        #region HTTP POST

        //Permet de rajouter un fournisseur
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Created, typeof(WriteSupplierViewModels), Description = "La création du fournisseur a été effectué avec succès")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(WriteSupplierViewModels), Description = "Le fichier Json est incorrect")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "Une ou plusieurs valeurs dépassent le nombre de caractère autorisé")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteSupplierViewModels> CreateSupplier(WriteSupplierViewModels supplier)
        {
            //On vérifie que le JSON reçu est conforme au WriteModels
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Supplier finalSupplier = new Supplier();

            finalSupplier.Name = supplier.Name;
            finalSupplier.Adress = supplier.Adress;
            finalSupplier.City = supplier.City;
            finalSupplier.ZipCode = supplier.ZipCode;
            finalSupplier.Phone = supplier.Phone;
            finalSupplier.Mail = supplier.Mail;


            _context.Supplier.Add(finalSupplier);
            _context.SaveChanges();

            return CreatedAtRoute("GetSupplier", new { idSupplier = finalSupplier.Id }, finalSupplier);
        }

        #endregion

        #region HTTP PUT

        [HttpPut("{supplierId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(WriteSupplierViewModels), Description = "La mise à jour du fournisseur a été effectué avec succès")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(WriteSupplierViewModels), Description = "Le fichier Json est incorrect")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "Une ou plusieurs valeurs dépassent le nombre de caractère autorisé")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID du fournisseur renseigné est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteSupplierViewModels> UpdateSupplier(int supplierId, WriteSupplierViewModels supplier)
        {
            //On vérifie que le JSON reçu est conforme au ViewModels
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var supplierFromDataBase = _context.Supplier.Single(s => s.Id == supplierId);
            if (supplier == null)
            {
                return NotFound();
            }

            supplierFromDataBase.Name = supplier.Name;
            supplierFromDataBase.Adress = supplier.Adress;
            supplierFromDataBase.City = supplier.City;
            supplierFromDataBase.ZipCode = supplier.ZipCode;
            supplierFromDataBase.Phone = supplier.Phone;
            supplierFromDataBase.Mail = supplier.Mail;

            _context.Entry(supplierFromDataBase).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok();
        }

        #endregion

        #region HTTP PATCH

        [HttpPatch("{supplierId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(EmptyResult), Description = "La mise à jour partiel du fournisseur a été effectué avec succès")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "Le fichier Json est incorrect")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "La modification dépasse le nombre de caractère autorisé")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID du fournisseur est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteSupplierViewModels> PatchSupplier(int supplierId, JsonPatchDocument<WriteSupplierViewModels> patchDoc)
        {
            //Récupération du fournisseur en fonction de l'ID
            var supplierFromDataBase = _context.Supplier.Single(s => s.Id == supplierId);
            if (supplierFromDataBase == null)
            {
                return NotFound();
            }

            //Creation du fournisseur temporaire avec les infos déjà existante
            var supplierToPatch = new WriteSupplierViewModels()
            {
                Name = supplierFromDataBase.Name,
                Adress = supplierFromDataBase.Adress,
                City = supplierFromDataBase.City,
                ZipCode = supplierFromDataBase.ZipCode,
                Phone = supplierFromDataBase.Phone,
                Mail = supplierFromDataBase.Mail,

            };

            //Application des modifications dans le fournisseur temporaire avec vérification du ModelState
            patchDoc.ApplyTo(supplierToPatch, ModelState);

            //Si le modèle n'est pas respecté avec ERREUR
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            //Vérifie que le ModelState est correcte pour les valeur fournisseur Temporaire
            if (!TryValidateModel(supplierToPatch))
            {
                return BadRequest(ModelState);
            }

            //Application du résultat dans la BDD
            supplierFromDataBase.Name = supplierToPatch.Name;
            supplierFromDataBase.Adress = supplierToPatch.Adress;
            supplierFromDataBase.City = supplierToPatch.City;
            supplierFromDataBase.ZipCode = supplierToPatch.ZipCode;
            supplierFromDataBase.Phone = supplierToPatch.Phone;
            supplierFromDataBase.Mail = supplierToPatch.Mail;

            _context.SaveChanges();

            return Ok();

        }

        #endregion

        #region HTTP DELETE

        [HttpDelete("{supplierId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(EmptyResult), Description = "La suppression du fournisseur a été effectué avec succès")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID de du fournisseur est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(EmptyResult), Description = "Présence de clé étangère, impossible de supprimer le fournisseur")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteSupplierViewModels> DeleteSupplier(int supplierId)
        {
            //Récupération du fournisseur en fonction de l'ID
            var supplierFromDataBase = _context.Supplier.Single(s => s.Id == supplierId);
            if (supplierFromDataBase == null)
            {
                return NotFound();
            }

            try
            {
                //Si il existe alors on peut supprimer
                _context.Supplier.Remove(supplierFromDataBase);
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
