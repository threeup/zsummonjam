using UnityEngine;

namespace zapo
{
    public static class ZapoDataHelper
    {
        private static string[] RandomNames = {
            "Sleve McDaniel",
            "Jannifer McDaniel",
            "Onson Sweemey",
            "Alanda Sweemey",
            "Darryl Archideld",
            "Jossica Archideld",
            "Anatoli Smlorin",
            "Meblissa Smlorin",
            "Rey McSriff",
            "Darah McSriff",
            "Glenallen Mixon",
            "Heather Mixon",
            "Mario McRlwain",
            "Nicole McRlwain",
            "Raul Chamgerlain",
            "Ammy Chamgerlain",
            "Kevin Nogilny",
            "Ebizaleth Nogilny",
            "Tony Smehrik",
            "Michello Smehrik",
            "Bobson Dugnutt",
            "Crysbal Dugnutt",
            "Willie Dustice",
            "Ashrey Dustice",
            "Jeromy Gride",
            "Blittany Gride",
            "Scott Dourque",
            "Samontha Dourque",
            "Shown Furcotte",
            "Lauren Furcotte",
            "Dean Wesrey",
            "Sally Wesrey",
            "Mike Truk",
            "Lucy Truk",
            "Dwigt Rortugal",
            "Pafty Rortugal",
            "Tim Sandaele",
            "Larcie Sandaele",
            "Karl Dandleton",
            "Bella Dandleton",
            "Mike Sernandez",
            "Yarsmine Sernandez",
            "Todd Bonzalez",
            "Anza Bonzalez"
        };

        public static string GetRandomName()
        {
            int k = Random.Range(0, RandomNames.Length);
            return RandomNames[k];
        }
    }
}