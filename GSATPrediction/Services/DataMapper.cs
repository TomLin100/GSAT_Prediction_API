using GSATPrediction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GSATPrediction.Services
{
    public class DataMapper
    {
        private SignUpViewModel _signUp;

        public DataMapper(SignUpViewModel sign)
        {
            _signUp = sign;
        }

        public UserHistory Mapper()
        {
            var history = new UserHistory()
            {
                id = Guid.NewGuid(),
                email = _signUp.email,
                phone = _signUp.phoneNumber,
                address = _signUp.address,
                Hlocation = _signUp.location,
                identities = _signUp.identity,
                schoolName = _signUp.schoolName,
                isApplyCHU = _signUp.isApplyCHU ? 1 : 0,
                interestedDepart = _signUp.interestedDepart,
                gsat_Chinese = Convert.ToInt32(_signUp.user_input.grades.gsat.Chinese),
                gsat_English = Convert.ToInt32(_signUp.user_input.grades.gsat.English),
                gsat_Math = Convert.ToInt32(_signUp.user_input.grades.gsat.Math),
                gsat_Science = Convert.ToInt32(_signUp.user_input.grades.gsat.Science),
                gsat_Society = Convert.ToInt32(_signUp.user_input.grades.gsat.Society),
                ELlevel = _signUp.user_input.grades.gsat.EngListeningLevel,
                Ulocation = MapToString(_signUp.user_input.location),
                UGroup = MapToString(_signUp.user_input.groups),
                property = MapToString(_signUp.user_input.property),
                salary = _signUp.user_input.expect_salary,
                createAt = DateTime.Now
            };

            return history;
        }

        private string MapToString(List<string> data)
        {
            string tmp = null;
            if (data.Count == 17 || data.Count == 19 || data.Count == 2)
            {
                return tmp = "全部";
            }
            else
            {
                foreach (var item in data)
                {
                    tmp += item + ",";
                }
                return tmp.TrimEnd(',');
            }
        }
    }
}
