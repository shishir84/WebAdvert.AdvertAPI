using AdvertApi.Models;
using AdvertApi.Services;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvertApi.Controllers
{
    [ApiController]
    [Route("adverts/v1")]
    [Produces("application/json")]
    public class AdvertController : Controller
    {
        private readonly IAdvertStorageService _advertStorageService;
        public IConfiguration Configuration { get; }

        public AdvertController(IAdvertStorageService advertStorageService, IConfiguration configuration)
        {
            _advertStorageService = advertStorageService;
            Configuration = configuration;
        }


        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(404)]
        [ProducesResponseType(201, Type = typeof(CreateAdvertResponse))]
        public async Task<IActionResult> Create(AdvertModel model)
        {
            string recordId;
            try
            {
                recordId = await _advertStorageService.AddAsync(model);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception.Message);
            }

            return StatusCode(201, new CreateAdvertResponse { Id = recordId });
        }

        [HttpPut]
        [Route("Confirm")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Confirm(ConfirmAdvertModel model)
        {
            try
            {
                await _advertStorageService.ConfirmAsync(model);
                //await RaiseAdvertConfirmedMessage(model);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception.Message);
            }

            return new OkResult();
        }

        //private async Task RaiseAdvertConfirmedMessage(ConfirmAdvertModel model)
        //{
        //    var topicArn = Configuration.GetValue<string>("TopicArn");
        //    var dbModel = await _advertStorageService.GetByIdAsync(model.Id);

        //    using (var client = new AmazonSimpleNotificationServiceClient())
        //    {
        //        var message = new AdvertConfirmedMessage
        //        {
        //            Id = model.Id,
        //            Title = dbModel.Title
        //        };

        //        var messageJson = JsonConvert.SerializeObject(message);
        //        await client.PublishAsync(topicArn, messageJson);
        //    }
        //}
    }
}
