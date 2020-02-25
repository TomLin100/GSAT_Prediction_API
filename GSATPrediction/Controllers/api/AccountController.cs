using GSATPrediction.Models;
using GSATPrediction.Services;
using GSATPrediction.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace GSATPrediction.Controllers.api
{
    [RoutePrefix("api/gsat")]
    public class AccountController : ApiController
    {
        private PredictEntities db = new PredictEntities();
        private SmsService sms = new SmsService();

        [Route("getValidateCode")]
        public HttpResponseMessage PostValidateCode([FromBody] ValidateInputViewModel validate)
        {
            HttpResponseMessage resp = null;
            var hasPhone = db.Validations.Find(validate.phone);
            if(hasPhone == null)
            {
                sms.PhoneNumber = validate.phone;
                var result = sms.send();
                var code = sms.responseFormat(result);
                var data = new Validation()
                {
                    phone = validate.phone,
                    code = sms.code
                };
                db.Validations.Add(data);
                try
                {
                    db.SaveChanges();
                    OutputViewModel output = new OutputViewModel()
                    {
                        status = Convert.ToInt32(HttpStatusCode.OK),
                        input = null,
                        validate = validate,
                        message = "發送成功，請至您的手機查看驗證碼"
                    };
                    resp = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ObjectContent<OutputViewModel>(output, new JsonMediaTypeFormatter())
                    };
                }
                catch (DbUpdateException ex)
                {
                    OutputViewModel output = new OutputViewModel()
                    {
                        status = Convert.ToInt32(HttpStatusCode.Conflict),
                        input = null,
                        validate = validate,
                        message = "此門號已被驗證過，無法再驗證"
                    };
                    resp = new HttpResponseMessage(HttpStatusCode.Conflict)
                    {
                        Content = new ObjectContent<OutputViewModel>(output, new JsonMediaTypeFormatter())
                    };
                }
            }
            else
            {
                OutputViewModel output = new OutputViewModel()
                {
                    status = Convert.ToInt32(HttpStatusCode.BadRequest),
                    input = null,
                    validate = validate,
                    message = "此門號已被驗證過，無法再驗證"
                };
                resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new ObjectContent<OutputViewModel>(output, new JsonMediaTypeFormatter())
                };
            }
            
            //if (code["smsStatus"] == 0)
            //{
            //    OutputViewModel output = new OutputViewModel()
            //    {
            //        status = HttpStatusCode.OK,
            //        input = null,
            //        validate = validate,
            //        message = "發送成功"
            //    };
            //    return Ok(output);
            //}
            //else
            //{
            //    return InternalServerError();
            //}

           
            return resp;
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
                        status = Convert.ToInt32(HttpStatusCode.OK),
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
                    status = Convert.ToInt32(HttpStatusCode.Created),
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
                //EmailService email = new EmailService();
                //email.ReceiveAddress = signUp.email;
                //email.send();

                OutputViewModel output = new OutputViewModel()
                {
                    status = Convert.ToInt32(HttpStatusCode.OK),
                    input = signUp,
                    validate=null,
                    message = "資料儲存成功"
                };
                return Ok(output);
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(ex.InnerException.Message);
            }           
        }
    }
}
