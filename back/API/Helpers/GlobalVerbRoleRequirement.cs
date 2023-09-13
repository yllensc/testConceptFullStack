using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace API.Helpers
{
    public class GlobalVerbRoleRequirement : IAuthorizationRequirement
    {
        public bool IsAllowed(string role, string verb){
            //admi
            if(string.Equals("Administrator", role, StringComparison.OrdinalIgnoreCase)) return true;
            if(string.Equals("Manager", role, StringComparison.OrdinalIgnoreCase)) return true;
            //GET
            //if(string.Equals("Employee", role, StringComparison.OrdinalIgnoreCase) && string.Equals("POST", verb, StringComparison.OrdinalIgnoreCase)) return true;
            if(string.Equals("Camper", role, StringComparison.OrdinalIgnoreCase) && string.Equals("GET", verb, StringComparison.OrdinalIgnoreCase)) return true;
            return false;

        }
    }
}