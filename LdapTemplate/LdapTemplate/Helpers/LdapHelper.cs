using LdapTemplate.Models;
using LdapTemplate.Results;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LdapTemplate.Helpers
{
    public static class LdapHelper
    {

        public static string username;
        public static string password;
        public static string host;
        public static List<string> hostArr;
        public static int port;
        public static string baseDn;
        public static string company;
        public static string mailDomain;
        private static int ldapVersion = Novell.Directory.Ldap.LdapConnection.LdapV3;

        public static string ComputeHashLDAP(string value)
        {
            var mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] numArray = mD5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(value));
            return Convert.ToBase64String(numArray, 0, numArray.Length);
        }

        public static Result<LdapEntry> Search(string value)
        {
            Result<LdapEntry> res = new Result<LdapEntry>();
            res.IsSuccess = false;
            res.ResultType = ResultType.Error;
            try
            {
                using (LdapConnection con = new LdapConnection())
                {
                    if (!string.IsNullOrEmpty(value))
                        foreach (var item in hostArr)
                        {
                            con.Connect(item, port);
                            con.Bind(ldapVersion, username, password);
                            Novell.Directory.Ldap.LdapSearchConstraints cons = con.SearchConstraints;
                            cons.ReferralFollowing = true;
                            con.Constraints = cons;
                            string searchFilter = $"(&(objectClass=user)(objectCategory=person)(sAMAccountName={value}))";
                            var lsc = con.Search(baseDn,
                                            Novell.Directory.Ldap.LdapConnection.ScopeSub,
                                            searchFilter,
                                            null,
                                            false,
                                            (LdapSearchConstraints)null);
                            var data = lsc.Next();
                            bool  checkAccisActive = data.GetAttribute("userAccountControl").StringValue == "512"  
                                || data.GetAttribute("userAccountControl").StringValue == "66048";
                            res.Data = data;
                            res.IsSuccess = true;
                            res.Message = "Arama işlemi başarılı";
                            res.ResultType = ResultType.Success;
                            return res;
                        }
                }
            }
            catch (Exception e)
            {
                res.Message = e.Message;
            }

            return res;
        }
       

        public static Result<DirectoryEntry> Add(LdapStudent student)
        {

            Result<DirectoryEntry> res = new Result<DirectoryEntry>();
            res.IsSuccess = false;
            res.ResultType = ResultType.Error;

            #region NewLDAPAttribute

            //try
            //{
            //    var attributeSet = new LdapAttributeSet
            //{

            //    new LdapAttribute("cn", student.OgrenciNo),
            //    new LdapAttribute("title", student.ProgramTuru),
            //    new LdapAttribute("givenName", student.OgrenciNo),
            //    new LdapAttribute("displayName", student.AdSoyad),
            //    new LdapAttribute("department", student.Fakulte),
            //    new LdapAttribute("company", company),
            //    new LdapAttribute("name", student.OgrenciNo),
            //    new LdapAttribute("sAMAccountName", student.OgrenciNo),
            //    new LdapAttribute("userPrincipalName", $"{student.OgrenciNo}@{mailDomain}"),
            //    new LdapAttribute("mail", $"{student.OgrenciNo}@{mailDomain}"),
            //    new LdapAttribute("GSM", student.GSM),
            //    new LdapAttribute("TCKimlik", student.TCKimlik),      
            //    //new LdapAttribute("userPassword", "{MD5}" + ComputeHashLDAP(student.OgrenciNo)),      
            //    //new LdapAttribute("userPassword", "{MD5}" + ComputeHashLDAP(student.OgrenciNo)), 
            //    new LdapAttribute("userpassword", student.OgrenciNo),

            //    new LdapAttribute("unicodePwd", Convert.ToBase64String(Encoding.Unicode.GetBytes($"\"{student.OgrenciNo}\""))),
            //    new LdapAttribute("userAccountControl", "66048"),

            //    new LdapAttribute("objectCategory", "CN=Person,CN=Schema,CN=Configuration,DC=IZTUSTU,DC=LOCAL"),
            //    new LdapAttribute("objectClass", new string[] { "top", "person", "organizationalPerson","user" })
            //};

            //    string dn = $"CN={student.OgrenciNo},{baseDn}";
            //    var newEntry = new LdapEntry(dn, attributeSet);
            //    //newUser.Invoke("SetPassword", new object[] { "pAssw0rdO1" });

            //    using (LdapConnection con = new LdapConnection())
            //    {
            //        foreach (var item in hostArr)
            //        {
            //            con.Connect(item, port);
            //            con.Bind(ldapVersion, username, password);
            //            con.Add(newEntry);
            //        }
            //        res.Data = newEntry;
            //        res.IsSuccess = true;
            //        res.ResultType = ResultType.Success;
            //        res.Message = "Başarılı";
            //    }
            //}
            //catch (Exception e)
            //{
            //    res.Message = e.Message;
            //}

            #endregion

            #region DirectoryEntry

            //Şifre kısmından dolayı bunu ekliyorum

            try
            {
                DirectoryEntry adEntry = new DirectoryEntry($"LDAP://{hostArr[0]}/{baseDn}", username, password);
                //adEntry.Path = $"LDAP://{baseDn}";              
                DirectoryEntry userEntry = adEntry.Children.Add($"CN={student.OgrenciNo}", "user");
                userEntry.Properties["cn"].Add(student.OgrenciNo);
                userEntry.Properties["title"].Add(student.ProgramTuru);
                userEntry.Properties["givenName"].Add(student.OgrenciNo);
                userEntry.Properties["displayName"].Add(student.AdSoyad);
                userEntry.Properties["department"].Add(student.Fakulte);
                userEntry.Properties["company"].Add(company);
                userEntry.Properties["name"].Add(student.OgrenciNo);
                userEntry.Properties["sAMAccountName"].Add(student.OgrenciNo);
                userEntry.Properties["userPrincipalName"].Add($"{student.OgrenciNo}@{mailDomain}");
                userEntry.Properties["mail"].Add($"{student.OgrenciNo}@{mailDomain}");
                userEntry.Properties["GSM"].Add(student.GSM);
                userEntry.Properties["TCKimlik"].Add(student.TCKimlik);
                userEntry.CommitChanges();
                //passworD123* test icin kabul ediyor.
                string generatedPassword = student.AdSoyad.First().ToString().ToUpper() + RemoveWhitespace(student.AdSoyad.Substring(1))+ "*" + student.TCKimlik.ToString();
                userEntry.Invoke("SetPassword", new object[] { "passworD123*" });
                userEntry.Properties["userAccountControl"].Value = "66048";
                userEntry.CommitChanges();               
                res.IsSuccess = true;
                res.ResultType = ResultType.Success;
                res.Message = "Başarılı";
            }
            catch (Exception ex)
            {
                string dn = $"CN ={ student.OgrenciNo},{baseDn}";
                Result<string> deleteResult = Delete(dn);
                res.Message = ex.InnerException != null ? ex.InnerException.ToString() :  ex.Message;
            }         


            #endregion


            return res;
        }

        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }




        public static Result<string> Delete(string dn)
        {
            Result<string> res = new Result<string>();
            res.IsSuccess = false;
            res.ResultType = ResultType.Error;
            try
            {                            
                using (LdapConnection con = new LdapConnection())
                {
                    foreach (var item in hostArr)
                    {
                        con.Connect(item, port);
                        con.Bind(ldapVersion, username, password);
                        con.Delete(dn);
                    }
                    res.Data = dn;
                    res.IsSuccess = true;
                    res.ResultType = ResultType.Success;
                    res.Message = "Başarılı";
                }
            }
            catch (Exception e)
            {
                res.Message = e.Message;
            }

            return res;
        }



        public static Result<string> ChangePassword(string studentNo, string newPassword)
        {
            DirectoryEntry searchRoot = null;
            DirectorySearcher searcher = null;
            DirectoryEntry userEntry = null;
            Result<string> res = new Result<string>();
            res.IsSuccess = false;
            res.ResultType = ResultType.Error;
            try
            {
                searchRoot = new DirectoryEntry($"LDAP://{hostArr[0]}/{baseDn}", username, password);
                searcher = new DirectorySearcher(searchRoot);
                searcher.Filter = String.Format("sAMAccountName={0}", studentNo);
                searcher.SearchScope = SearchScope.Subtree;
                searcher.CacheResults = false;

                SearchResult searchResult = searcher.FindOne(); ;
                if (searchResult != null)
                {

                    userEntry = searchResult.GetDirectoryEntry();

                    userEntry.Invoke("SetPassword", new object[] { newPassword });
                    userEntry.CommitChanges();

                    res.IsSuccess = true;
                    res.ResultType = ResultType.Success;
                    res.Message = "Başarılı";
                }
                else
                {
                    res.Message = "Kullanici Bulunamadi";
                }
            }
            catch (Exception ex)
            {
                res.Message = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
            }
            finally
            {
                if (userEntry != null) userEntry.Dispose();
                if (searcher != null) searcher.Dispose();
                if (searchRoot != null) searchRoot.Dispose();
            }
            return res;
        }

    }
}
