using LdapTemplate.Helpers;
using LdapTemplate.Models;
using LdapTemplate.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Threading.Tasks;

namespace LdapTemplate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LdapController : ControllerBase
    {
        public IConfiguration _configuration { get; set; }
        public LdapController(IConfiguration configuration)
        {            
            _configuration = configuration;
            LdapHelper.username = configuration["LDAP:Username"];
            LdapHelper.password = configuration["LDAP:Password"];
            LdapHelper.host = configuration["LDAP:Host"];
            LdapHelper.hostArr = LdapHelper.host.Split(';').ToList();
            LdapHelper.port = Convert.ToInt32(configuration["LDAP:Port"]);
            LdapHelper.baseDn = configuration["LDAP:BaseDn"];    
            LdapHelper.company = configuration["LDAP:Company"]; 
            LdapHelper.mailDomain = configuration["LDAP:MailDomain"];
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("search/{value}")]
        public IActionResult Search(string value)
        {
            Result<LdapEntry> res = LdapHelper.Search(value);
            if (res.IsSuccess)
                return Ok(res);
            return NotFound(res);
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("add")]
        public IActionResult Add(LdapStudent student)
        {
            Result<DirectoryEntry> addResult = new Result<DirectoryEntry>();
            addResult.IsSuccess = false;
            addResult.ResultType = ResultType.Error;
            Result<LdapEntry> searchResult = LdapHelper.Search(student.OgrenciNo);
            if (searchResult.IsSuccess)
            {
                addResult.Message = "Kayıt Daha Önceden Eklenmiş";
                addResult.Data = null;
                return BadRequest(addResult);
            }

            addResult = LdapHelper.Add(student);
            if (searchResult.IsSuccess)
                return Ok(addResult);
             return BadRequest(addResult);
        }


        [Microsoft.AspNetCore.Mvc.HttpGet("delete/{value}")]
        public IActionResult Delete(string value)
        {
            Result<LdapEntry> res = LdapHelper.Search(value);
            Result<string> resultDelete = new Result<string>();
            if (res.IsSuccess)
            {
               resultDelete = LdapHelper.Delete(res.Data.Dn.ToString());
                if (res.IsSuccess)
                    return Ok(resultDelete);
                return BadRequest(resultDelete);
            }
            return NotFound(res);
            
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("validate/{value}")]
        public IActionResult Validate(ValidateModel userForValidate)
        {
            Result<string> res = LdapHelper.Validate(userForValidate.KullaniciAdi, userForValidate.Sifre);
            if (res.IsSuccess)
            {
                return Ok(res);
            }
            return NotFound(res);

        }


    }
}
