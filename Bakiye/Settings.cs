using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bakiye
{
    public class Settings
    {
        public static readonly string cs = ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;

        public static List<SelectListItem> KasalarDropdown()
        {
            List<SelectListItem> liste = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand kasagetir = new SqlCommand(@"select * from Kasa", con))
                {
                    using (SqlDataReader dr = kasagetir.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SelectListItem slitem = new SelectListItem
                            {
                                Value = dr["ID"].ToString(),
                                Text = dr["KasaAdi"].ToString(),
                            };
                            liste.Add(slitem);
                        }
                    }
                }
            }
            return liste;
        }// Kasalar Dropdown

        public static List<SelectListItem> BankalarDropdown()
        {
            List<SelectListItem> liste = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand bankagetir = new SqlCommand(@"select * from Banka", con))
                {
                    using (SqlDataReader dr = bankagetir.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            SelectListItem slitem = new SelectListItem
                            {
                                Value = dr["ID"].ToString(),
                                Text = dr["BankaAdi"].ToString(),
                            };
                            liste.Add(slitem);
                        }
                    }
                }
            }
            return liste;
        }// Bankalar Dropdown
    }
}