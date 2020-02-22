using GSATPrediction.Models;
using GSATPrediction.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GSATPrediction.Controllers.api
{
    [RoutePrefix("api/gsat")]
    public class AccountController : ApiController
    {
        private PredictEntities db = new PredictEntities();

        [Route("store")]
        public IHttpActionResult Post([FromBody] SignUpViewModel signUp)
        {
            try
            {
                DataMapper data = new DataMapper(signUp);
                UserHistory user = data.Mapper();
                db.UserHistories.Add(user);
                db.SaveChanges();

                // TODO: 寄信會影響使用者體驗
                EmailService email = new EmailService();
                email.ReceiveAddress = signUp.email;
                email.send();

                OutputViewModel output = new OutputViewModel()
                {
                    status = HttpStatusCode.OK,
                    input = signUp,
                    message = "資料儲存成功"
                };
                return Ok(output);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }           
        }
    }
}
