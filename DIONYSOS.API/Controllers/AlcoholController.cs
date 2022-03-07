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
    [Route("api/alcohols")]
    [Produces("application/json")]
    [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(EmptyResult), Description = "Veuillez vous authentifier à l'API")]
    [SwaggerResponse(HttpStatusCode.Forbidden, typeof(EmptyResult), Description = "Vous n'avez pas les privillèges nécessaires")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class AlcoholController : ControllerBase
    {
        //Ajout du contructeur puis injection du context pour la connexion à la BDD
        private readonly DionysosContext _context;

        public AlcoholController(DionysosContext context)
        {
            _context = context;
        }

        #region HTTP GET

        //Récupération de tous les alcools
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ReadAlcoholViewModels), Description = "La récupération de tous les alcools avec le produit associé a été un succès")]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(EmptyResult), Description = "La table alcool est vide")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<ReadAlcoholViewModels>> GetAlcohols()
        {
            //Si la table est vide
            if (!_context.Alcohol.Any())
            {
                return NoContent();
            }

            var AlcoholSupplierLink = _context.Alcohol
                .Join(
                    _context.Product,
                    Alcohol => Alcohol.Product.Id,
                    Product => Product.Id,
                    (Alcohol, Product) => new ReadAlcoholViewModels()
                    {
                        Id = Alcohol.Id,
                        GrapeVariety = Alcohol.GrapeVariety,
                        Vintage = Alcohol.Vintage,
                        Organic = Alcohol.Organic,
                        Place = Alcohol.Place,
                        Keeping = Alcohol.Keeping,
                        Color = Alcohol.Color,
                        Pairing = Alcohol.Pairing,
                        ProductId = Product.Id,
                        ProductName = Product.Name,
                        ProductBarCode = Product.BarCode,
                        ProductPrice = Product.Price,
                        ProductDescription = Product.Description,
                        ProductState = Product.State,
                        ProductQuantity = Product.Quantity,
                        ProductQuantityMax = Product.QuantityMax,
                        ProductSite = Product.Site

                    }
                ).ToList();



            return Ok(AlcoholSupplierLink);
        }

        //Récupération d'un alcool en fonction de l'ID de l'alcool
        [HttpGet("{alcoholId}", Name = "GetAlcohol")]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ReadAlcoholViewModels), Description = "La récupération de l'alcool via l'id avec le produit associé réussi")]
        [SwaggerResponse(HttpStatusCode.NoContent, typeof(EmptyResult), Description = "La table alcool est vide")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID de l'alcool est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ReadAlcoholViewModels> GetAlcoholById(int alcoholId)
        {
            //Si la table est vide
            if (!_context.Alcohol.Any())
            {
                return NoContent();
            }

            //Vérification si l'alcool avec l'id renseigné existe
            var alcoholToFind = _context.Alcohol.Find(alcoholId);
            if (alcoholToFind == null)
            {
                return NotFound();
            }

            var AlcoholSupplierLink = _context.Alcohol
                .Where(a => a.Id == alcoholId)
                .Join(
                    _context.Product,
                    Alcohol => Alcohol.Product.Id,
                    Product => Product.Id,
                    (Alcohol, Product) => new ReadAlcoholViewModels()
                    {
                        Id = Alcohol.Id,
                        GrapeVariety = Alcohol.GrapeVariety,
                        Vintage = Alcohol.Vintage,
                        Organic = Alcohol.Organic,
                        Place = Alcohol.Place,
                        Keeping = Alcohol.Keeping,
                        Color = Alcohol.Color,
                        Pairing = Alcohol.Pairing,
                        ProductId = Product.Id,
                        ProductName = Product.Name,
                        ProductBarCode = Product.BarCode,
                        ProductPrice = Product.Price,
                        ProductDescription = Product.Description,
                        ProductState = Product.State,
                        ProductQuantity = Product.Quantity,
                        ProductQuantityMax = Product.QuantityMax,
                        ProductSite = Product.Site


                    }
                ).Single();

            return Ok(AlcoholSupplierLink);
        }

        #endregion

        #region HTTP POST

        //Permet de rajouter un alcool avec l'ID d'un produit
        [HttpPost("products/{productId}")]
        [SwaggerResponse(HttpStatusCode.Created, typeof(WriteAlcoholViewModels), Description = "La création de l'alcool a été effectué avec succès")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(WriteAlcoholViewModels), Description = "Le fichier Json est incorrect")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "Une ou plusieurs valeurs dépassent le nombre de caractère autorisé")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID du produit renseigné est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteAlcoholViewModels> CreateAlcohol(int productId, WriteAlcoholViewModels alcohol)
        {
            //On vérifie que le JSON reçu est conforme au WriteModels
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Alcohol finalAlcohol = new Alcohol();

            var product = _context.Product.Find(productId);
            if (product == null)
            {
                return NotFound();
            }

            finalAlcohol.Product = product;
            finalAlcohol.GrapeVariety = alcohol.GrapeVariety;
            finalAlcohol.Vintage = alcohol.Vintage;
            finalAlcohol.Organic = alcohol.Organic;
            finalAlcohol.Place = alcohol.Place;
            finalAlcohol.Keeping = alcohol.Keeping;
            finalAlcohol.Color = alcohol.Color;
            finalAlcohol.Pairing = alcohol.Pairing;


            _context.Alcohol.Add(finalAlcohol);
            _context.SaveChanges();

            return CreatedAtRoute("GetAlcohol", new { alcoholId = finalAlcohol.Id }, finalAlcohol);
        }

        #endregion

        #region HTTP PUT

        [HttpPut("{alcoholId}/products/{productId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(WriteAlcoholViewModels), Description = "La mise à jour de l'alcool a été effectué avec succès")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(WriteAlcoholViewModels), Description = "Le fichier Json est incorrect")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "Une ou plusieurs valeurs dépassent le nombre de caractère autorisé")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID du produit ou de l'alcool renseigné est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteAlcoholViewModels> UpdateAlcohol(int alcoholId, int productId, WriteAlcoholViewModels alcohol)
        {
            //On vérifie que le JSON reçu est conforme au ViewModels
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var AlcoholFromDataBase = _context.Alcohol.Single(a => a.Id == alcoholId);
            if (alcohol == null)
            {
                return NotFound();
            }

            var product = _context.Product.Find(productId);
            if (product == null)
            {
                return NotFound();
            }

            AlcoholFromDataBase.Product = product;
            AlcoholFromDataBase.GrapeVariety = alcohol.GrapeVariety;
            AlcoholFromDataBase.Vintage = alcohol.Vintage;
            AlcoholFromDataBase.Organic = alcohol.Organic;
            AlcoholFromDataBase.Place = alcohol.Place;
            AlcoholFromDataBase.Keeping = alcohol.Keeping;
            AlcoholFromDataBase.Color = alcohol.Color;
            AlcoholFromDataBase.Pairing = alcohol.Pairing;

            _context.Entry(AlcoholFromDataBase).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok();
        }

        #endregion

        #region HTTP PATCH
        //HTTP PATCH : Modification partielle
        [HttpPatch("{alcoholId}/products/{productId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(EmptyResult), Description = "La mise à jour partiel de l'alcool a été effectué avec succès")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "Le fichier Json est incorrect")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(EmptyResult), Description = "La modification dépasse le nombre de caractère autorisé")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID du produit ou de l'alcool renseigné est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json-patch+json")]
        public ActionResult<WriteProductViewModels> PatchAlcohol(int alcoholId, int productId, JsonPatchDocument<WriteAlcoholViewModels> patchDoc)
        {
            //On vérifie que le produit existe
            var product = _context.Product.Find(productId);
            if (product == null)
            {
                return NotFound();
            }

            //Récupération du fournisseur en fonction de l'ID
            var AlcoholFromDataBase = _context.Alcohol.Single(a => a.Id == alcoholId);
            if (AlcoholFromDataBase == null)
            {
                return NotFound();
            }

            //Creation du fournisseur temporaire avec les infos déjà existante
            var alcoholToPatch = new WriteAlcoholViewModels()
            {

                GrapeVariety = AlcoholFromDataBase.GrapeVariety,
                Vintage = AlcoholFromDataBase.Vintage,
                Organic = AlcoholFromDataBase.Organic,
                Place = AlcoholFromDataBase.Place,
                Keeping = AlcoholFromDataBase.Keeping,
                Color = AlcoholFromDataBase.Color,
                Pairing = AlcoholFromDataBase.Pairing,
                ProductId = AlcoholFromDataBase.Product.Id

            };

            //Application des modifications dans l'alcool temporaire avec vérification du ModelState
            patchDoc.ApplyTo(alcoholToPatch, ModelState);

            //On vérifie que le JSON reçu est conforme au ViewModels
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            //Vérifie que le ModelState est correcte pour les valeur de l'alcool Temporaire
            if (!TryValidateModel(alcoholToPatch))
            {
                return BadRequest(ModelState);
            }

            //Application du résultat dans la BDD
            AlcoholFromDataBase.GrapeVariety = alcoholToPatch.GrapeVariety;
            AlcoholFromDataBase.Vintage = alcoholToPatch.Vintage;
            AlcoholFromDataBase.Organic = alcoholToPatch.Organic;
            AlcoholFromDataBase.Place = alcoholToPatch.Place;
            AlcoholFromDataBase.Keeping = alcoholToPatch.Keeping;
            AlcoholFromDataBase.Color = alcoholToPatch.Color;
            AlcoholFromDataBase.Pairing = alcoholToPatch.Pairing;
            AlcoholFromDataBase.Product.Id = alcoholToPatch.ProductId;

            _context.SaveChanges();

            return Ok();

        }

        #endregion

        #region HTTP DELETE

        [HttpDelete("{alcoholId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(EmptyResult), Description = "La suppression de l'alcool a été effectué avec succès")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(EmptyResult), Description = "L'ID de l'alcool est inconnu de la base de données")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(EmptyResult), Description = "Présence de clé étangère, impossible de supprimer l'alcool")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, typeof(EmptyResult), Description = "Erreur serveur interne")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<WriteProductViewModels> DeleteAlcohol(int alcoholId)
        {
            //Récupération d'un alcool en fonction de l'ID
            var alcoholFromDataBase = _context.Alcohol.Single(a => a.Id == alcoholId);
            if (alcoholFromDataBase == null)
            {
                return NotFound();
            }

            try
            {
                //Si il existe alors on le supprime
                _context.Alcohol.Remove(alcoholFromDataBase);
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
