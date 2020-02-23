using GSATPrediction.Models;
using GSATPrediction.Services;
using GSATPrediction.ViewModels;
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
        private SmsService sms = new SmsService();

        [Route("getValidateCode")]
        public IHttpActionResult PostValidateCode([FromBody] ValidateInputViewModel validate)
        {
            sms.PhoneNumber = validate.phone;
            var result = sms.send();
            var code = sms.responseFormat(result);
            if(code["smsStatus"] == 0)
            {
                OutputViewModel output = new OutputViewModel()
                {
                    status = HttpStatusCode.OK,
                    input = null,
                    validate = validate,
                    message = "發送成功"
                };
                return Ok(output);
            }
            else
            {
                return InternalServerError();
            }            
        }


        [Route("validate")]
        public IHttpActionResult PostValidate([FromBody] ValidateInputViewModel validate)
        {
            var data = db.Validations.Find(validate.phone);
            if(data == null)
            {
                return NotFound();
            }
            else if(data.code == validate.code)
            {
                data.isValidate = "Y";
                try
                {
                    db.SaveChanges();
                    OutputViewModel output = new OutputViewModel()
                    {
                        status = HttpStatusCode.OK,
                        input = null,
                        validate = validate,
                        message = "驗證成功"
                    };
                    return Ok(output);
                }
                catch (DbUpdateException ex)
                {
                    return InternalServerError(ex);
                }
            }
            else
            {
                OutputViewModel output = new OutputViewModel()
                {
                    status = HttpStatusCode.Created,
                    input = null,
                    validate = validate,
                    message = "驗證失敗"
                };
                return Ok(output);
            }
        }

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
