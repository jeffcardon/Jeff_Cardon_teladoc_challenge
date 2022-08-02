using System;
using System.Collections.Generic;
using System.Text;

namespace Teladoc
{
    class NewUserUi
    {
        public string addUserXpath = "//button[@class='btn btn-link pull-right']";
        public string firstNameFieldName = "FirstName";
        public string lastNameFieldName = "LastName";
        public string userNameFieldName = "UserName";
        public string passwordFieldName = "Password";
        public string customerRadioBttnsName = "optionsRadios";
        public string customerRadioBttnCompanyAXpath = "//Input[@value='15']";
        public string customerRadioBttnCompanyBXpath = "//Input[@value='16']";
        public string roleDropdownName = "RoleId";
        public string emailFieldName = "Email";
        public string cellPhoneFieldName = "Mobilephone";
        public string closeBttnXpath = "//button[@class='btn btn-danger']";
        public string saveBttnXpath = "//button[@class='btn btn-success']";
    }
}
