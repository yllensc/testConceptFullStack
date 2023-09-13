using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Domine.Entities;
using Domine.Interfaces;

namespace API.Helpers
{
    public class JWT
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double DurationOnMinutes { get; set; }

        private readonly IUnitOfWork _unitOfWork;
        public JWT(){

        }
        public JWT(IUnitOfWork unitOfWork){
            _unitOfWork = unitOfWork;
        }

        /* public dynamic validToken(ClaimsIdentity identity){
            try{
                    if(identity.Claims.Count() == 0){
                        return new {
                            success = false,
                            message = "token no válido",
                            result = ""
                        };
                    }
                    var id = identity.Claims.FirstOrDefault(i => i.Type == "uid").Value;
                    var user =  _unitOfWork.Users.GetSomeUserLogic(id);
                    if(user !=null){
                        return new {
                            success = true,
                            message = "token válido, estás autorizado",
                            result = user
                        };
                    }
                    else{
                         return new {
                            success = false,
                            message = "Usuario no encontrado",
                            result = ""
                        }; 
                    }
            }
            catch{
                
                return new {
                            success = false,
                            message = "token no válido, error inesperado jeje",
                            result = ""
                        };

            }
        } */
    }
}