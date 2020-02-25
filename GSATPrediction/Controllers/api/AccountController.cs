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
            if (hasPhone == null)
            {
                sms.PhoneNumber = validate.phone;
                var result = sms.send();
                //var code = sms.responseFormat(result);
                var data = new Validation()
                {
                    phone = validate.phone,
                    code = sms.code,
                    isValidate = "N",
                    createAt = DateTime.Now
                };

                try
                {
                    db.Validations.Add(data);
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
            else if (hasPhone != null && hasPhone.isValidate != "N")
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
            else
            {
                OutputViewModel output = new OutputViewModel()
                {
                    status = Convert.ToInt32(HttpStatusCode.Conflict),
                    input = null,
                    validate = validate,
                    message = "簡訊驗證碼已經產生，請確認您的簡訊驗證碼"
                };
                resp = new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new ObjectContent<OutputViewModel>(output, new JsonMediaTypeFormatter())
                };
            }
            return resp;
        }


        [Route("validate")]
        public HttpResponseMessage PostValidate([FromBody] ValidateInputViewModel validate)
        {
            HttpResponseMessage resp = null;
            var data = db.Validations.Find(validate.phone);
            if (data == null)
            {
                OutputViewModel output = new OutputViewModel()
                {
                    status = Convert.ToInt32(HttpStatusCode.NotFound),
                    input = null,
                    validate = validate,
                    message = "找不到手機門號，請確認您有取得簡訊驗證碼再行驗證"
                };
                resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new ObjectContent<OutputViewModel>(output, new JsonMediaTypeFormatter())
                };
                return resp;
            }
            else if (data.code == validate.code && data.isValidate == "N")
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
                    resp = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ObjectContent<OutputViewModel>(output, new JsonMediaTypeFormatter())
                    };
                    return resp;
                }
                catch (DbUpdateException ex)
                {
                    OutputViewModel output = new OutputViewModel()
                    {
                        status = Convert.ToInt32(HttpStatusCode.InternalServerError),
                        input = null,
                        validate = validate,
                        message = ex.Message
                    };
                    resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new ObjectContent<OutputViewModel>(output, new JsonMediaTypeFormatter())
                    };
                    return resp;
                }
            }else if(data.code != validate.code && data.isValidate == "N")  //驗證碼不對
            {
                OutputViewModel output = new OutputViewModel()
                {
                    status = Convert.ToInt32(HttpStatusCode.BadRequest),
                    input = null,
                    validate = validate,
                    message = "驗證碼錯誤，請再確認您手機簡訊中的驗證碼"
                };
                resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new ObjectContent<OutputViewModel>(output, new JsonMediaTypeFormatter())
                };
                return resp;
            }
            else
            {
                OutputViewModel output = new OutputViewModel()
                {
                    status = Convert.ToInt32(HttpStatusCode.Unauthorized),
                    input = null,
                    validate = validate,
                    message = "此門號已被驗證過，無法再驗證"
                };
                resp = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new ObjectContent<OutputViewModel>(output, new JsonMediaTypeFormatter())
                };
                return resp;
            }
        }

        [Route("store")]
        public HttpResponseMessage PostStore([FromBody] SignUpViewModel signUp)
        {
            HttpResponseMessage resp = null;
            DataMapper data = new DataMapper(signUp);
            UserHistory user = data.Mapper();

            //驗證使用者的email是否已經存在於DB中
            var hasEmail = db.UserHistories.Find(signUp.email);
            if (hasEmail != null)  //有的話，丟409 conflic去跟前端說
            {
                OutputViewModel output = new OutputViewModel()
                {
                    status = Convert.ToInt32(HttpStatusCode.Conflict),
                    input = signUp,
                    validate = null,
                    message = "您已經輸入過您的電子郵件信箱"
                };
                resp = new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new ObjectContent<OutputViewModel>(output, new JsonMediaTypeFormatter())
                };
                return resp;
            }
            else   //沒有的話，加入資料庫之後寄信給使用者(200 OK)
            {
                try
                {
                    db.UserHistories.Add(user);
                    db.SaveChanges();

                    // TODO: 寄信會影響使用者體驗
                    EmailService email = new EmailService();
                    email.ReceiveAddress = signUp.email;
                    email.send();

                    OutputViewModel output = new OutputViewModel()
                    {
                        status = Convert.ToInt32(HttpStatusCode.OK),
                        input = signUp,
                        validate = null,
                        message = "已成功輸入資料並寄出免費推甄必勝秘笈，請查看您的電子郵件信箱"
                    };
                    resp = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ObjectContent<OutputViewModel>(output, new JsonMediaTypeFormatter())
                    };
                    return resp;
                }
                catch (Exception ex)
                {
                    OutputViewModel output = new OutputViewModel()
                    {
                        status = Convert.ToInt32(HttpStatusCode.InternalServerError),
                        input = signUp,
                        validate = null,
                        message = ex.Message
                    };
                    resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new ObjectContent<OutputViewModel>(output, new JsonMediaTypeFormatter())
                    };
                    return resp;
                }
            }                        
        }
    }
}
