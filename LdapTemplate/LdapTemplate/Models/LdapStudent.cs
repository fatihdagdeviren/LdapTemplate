using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LdapTemplate.Models
{

    //   {
    //  "adSoyad": "Unipa Test",
    //  "tcKimlik": "11111111111",
    //  "gsm": "05555555555",
    //  "ogrenciNo": "11111111111",
    //  "programTuru": "Lisans",
    //  "fakulte": "TIP Fakultesi"
    //}

public class LdapStudent
    {
       
        public string AdSoyad { get; set; }
        public string TCKimlik { get; set; }
        public string GSM { get; set; }
        public string OgrenciNo { get; set; }
        public string ProgramTuru { get; set; }
        public string Fakulte { get; set; }
    }
}
