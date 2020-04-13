using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Profiles;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CourseLibrary.API.Controllers
{
    [Route("api/authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public AuthorsController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        //[HttpGet("({ids})")]
        //public IActionResult GetAuthorCollection([FromRoute] IEnumerable<string> ids)
        //{

        //}

        [HttpGet]
        public IActionResult GetAuthors(string mainCategory, string searchQuery)
        {
            return Ok(_mapper.Map <IEnumerable<AuthorDto>>(_courseLibraryRepository.GetAuthors(mainCategory, searchQuery)));
        }

        [Produces("application/json",
            "application/vnd.khadoos.full+json",
            "application/vnd.khadoos.friendly+json")]
        [HttpGet("{authorId}", Name ="GetAuthor")]
        public IActionResult GetAuthor(string authorId, [FromHeader(Name ="Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
                return BadRequest();

            var author = _courseLibraryRepository.GetAuthor(ObjectId.Parse(authorId));

            if (author == null)
                return NotFound();

            if (parsedMediaType.SubTypeWithoutSuffix == "vnd.khadoos.full")
                return Ok(_mapper.Map<AuthorFullDto>(author));

            return Ok(_mapper.Map<AuthorDto>(author));
        }

        [HttpPost]
        public ActionResult<AuthorDto> CreateAuthor(AuthorDtoForCreation model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var authorEntity = _mapper.Map<Author>(model);
            _courseLibraryRepository.CreateAuthor(authorEntity);

            var authorDto = _mapper.Map<AuthorDto>(authorEntity);

            return CreatedAtRoute("GetAuthor", new { authorId = authorDto.Id }, authorDto);
        }

        [HttpPut("{authorId}")]
        public ActionResult<AuthorDto> UpdateAuthor(string authorId, AuthorForUpdate model)
        {
            var authorFromRepo = _courseLibraryRepository.GetAuthor(ObjectId.Parse(authorId));
            if (authorFromRepo == null)
                return NotFound();
            _mapper.Map(model, authorFromRepo);
           
            _courseLibraryRepository.UpdateAuthor(ObjectId.Parse(authorId), authorFromRepo);

            return Ok(_mapper.Map<AuthorDto>(authorFromRepo));
        }


        [HttpPatch("{authorId}")]
        public ActionResult PartiallyUpdateAuthor(string authorId, JsonPatchDocument<AuthorDtoForCreation> patchDocument)
        {
            var authorFromRepo = _courseLibraryRepository.GetAuthor(ObjectId.Parse(authorId));

            if (authorFromRepo == null)
                return NotFound();

            var authorToPatch = _mapper.Map<AuthorDtoForCreation>(authorFromRepo);

            //Add Validation
            //trying to do operation on the field that doesn't exist give error 500 error but it should be 400 and to do that we pass ModelState to make it invalid
            patchDocument.ApplyTo(authorToPatch, ModelState);  //patch document error made the model state invalid

            //once the patch update apply to authorPatch we can check the manually if author patch is still valid for example we didn't remove the required field
            TryValidateModel(authorToPatch);

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            _mapper.Map(authorToPatch, authorFromRepo);

            _courseLibraryRepository.UpdateAuthor(ObjectId.Parse(authorId), authorFromRepo);

            return NoContent();
        }

        [HttpDelete("{authorId}")]
        public ActionResult DeleteAuthor(string authorId)
        {
            var authorFromRepo = _courseLibraryRepository.GetAuthor(ObjectId.Parse(authorId));

            if (authorFromRepo == null)
                return NotFound();

            _courseLibraryRepository.DeleteAuthor(ObjectId.Parse(authorId));

            return NoContent();
        }

        //Use the custom cofigure Invalid model state response factory in case of model is invalid
        public override ActionResult ValidationProblem(ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices.GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
        

    }

   
} 