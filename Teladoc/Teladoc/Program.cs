using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System;

namespace Teladoc
{
    class Program
    {
        public static IWebDriver _Driver;
        public enum _By { className, cssSelector, id, linkText, name, partialLinkText, tagName, xPath }
        public enum _CustomerType { CompanyAAA, CompanyBBB, None }
        public enum _RoleType { SalesTeam, Customer, Admin}

        static void Main(string[] args)
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("start-maximized");
            _Driver = new ChromeDriver(options);
            _Driver.Navigate().GoToUrl("http://www.way2automation.com/angularjs-protractor/webtables/");

            if (!WaitForElement("//button[@class='btn btn-link pull-right']", _By.xPath, 90))
            {
                Console.WriteLine("Unable to launch site: http://www.way2automation.com/angularjs-protractor/webtables/");
                return;
            }

            #region user variables
            // Add new user
            string testType = args[0];
            if (!args[0].ToLower().Contains("add") && !args[0].ToLower().Contains("delete"))
            {
                Console.WriteLine("Nothing to do!");
                return;
            }

            if (args.Length < 9)
                Console.WriteLine("Missing information!");

            string firstName = args[1];      // Required
            string username = args[2];  // Required
            _RoleType role = _RoleType.Customer;    // Required
            string cellphone = args[4]; // Required
            string lastName = "";  // Optional
            string password = "";   //Optional
            _CustomerType customer = _CustomerType.None; // Optional
            string email = "";     // Optional
            switch(args[3].ToLower())
            {
                case "Customer":
                    role = _RoleType.Customer;
                    break;
                case "Admin": case "Administrator":
                    role = _RoleType.Admin;
                    break;
                case "SalesTeam": case "Sales Team":
                    role = _RoleType.SalesTeam;
                    break;
                default:
                    role = _RoleType.Customer;
                    break;
            }

            try
            {
                if (args[5] != "")
                    lastName = args[5];
                if (args[6] != "")
                    password = args[6];
                if (args[8] != "")
                    email = args[8];
                if (args[7] != "")
                {
                    switch (args[7])
                    {
                        case "CompanyA":
                        case "CompanyAA":
                        case "CompanyAAA":
                            customer = _CustomerType.CompanyAAA;
                            break;
                        case "CompanyB":
                        case "CompanyBB":
                        case "CompanyBBB":
                            customer = _CustomerType.CompanyBBB;
                            break;
                        default:
                            customer = _CustomerType.CompanyAAA;
                            break;
                    }
                }
            }
            catch
            {
                // Ignore exception, these are optional details
            }

            #endregion

            if(testType.ToLower().Contains("add"))
            {
                // Add customer
                AddNewUser(firstName, username, role, cellphone, lastName, password, customer, email);

                // Validate customer was added
                if (VerifyRowAddedToTable(firstName, role.ToString()))
                    Console.WriteLine("\r\n*** Customer: " + firstName + " added successfully. ***\r\n");
            }

            if(testType.ToLower().Contains("delete"))
            {
                DeleteUserFromTable(firstName, username, role.ToString(), cellphone);

                if (VerifyDeletedFromTable(firstName, role.ToString()))
                    Console.WriteLine("\r\n*** Customer " + firstName + " successfully removed from table! ***\r\n");
            }
        }

        #region AddNewUser
        public static bool AddNewUser(
            string firstName,
            string userName,
            _RoleType role,
            string cellPhone,
            string lastName = "",
            string password = "",
            _CustomerType customer = _CustomerType.None,
            string email = "")
        {
            NewUserUi newUser = new NewUserUi();
            bool success = false;

            // Click the Add User button
            success = ClickFormBttn(newUser.addUserXpath);
            success = WaitForElement("FirstName", _By.name, 3);

            // Enter the first name
            if (success)
                success = EnterDataIntoTextField(newUser.firstNameFieldName, firstName);

            // Enter the last name
            if (success && lastName != "")
                success = EnterDataIntoTextField(newUser.lastNameFieldName, lastName);

            // Enter the user name
            if (success)
                success = EnterDataIntoTextField(newUser.userNameFieldName, userName);

            // Enter the password
            if (success && password != "")
                success = EnterDataIntoTextField(newUser.passwordFieldName, password);

            // Select the customer radio button
            if (success && customer != _CustomerType.None)
            {
                string customerXpath = "";

                if (customer == _CustomerType.CompanyAAA)
                    customerXpath = newUser.customerRadioBttnCompanyAXpath;
                else
                    customerXpath = newUser.customerRadioBttnCompanyBXpath;

                SelectCustomerRadioBttn(newUser.customerRadioBttnsName, customerXpath);
            }

            // Select the role
            if (success)
                SelectRole(newUser.roleDropdownName, role.ToString());

            // Enter the email
            if (success && email != "")
                success = EnterDataIntoTextField(newUser.emailFieldName, email);

            // Enter the cell phone
            if (success)
               success = EnterDataIntoTextField(newUser.cellPhoneFieldName, cellPhone);

            // Save
            if (success)
                success = ClickFormBttn(newUser.saveBttnXpath);

            return success;
        }
        #endregion
        #region ClickFormBttn
        public static bool ClickFormBttn(string element)
        {
            bool success = false;

            try
            {
                IWebElement formBttn = _Driver.FindElement(By.XPath(element));
                formBttn.Click();
                success = true;
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                return false;
            }

            return success;
        }
        #endregion
        #region DeleteUserFromTable
        public static bool DeleteUserFromTable(string firstName, string userName, string role, string cellPhone)
        {
            bool success = false;

            try
            {
                ReadOnlyCollection<IWebElement> rows = _Driver.FindElements(By.XPath("//tr[@ng-repeat='dataRow in displayedCollection']"));

                for(int i = 0; i < rows.Count; i++)
                {
                    ReadOnlyCollection<IWebElement> columns = rows[i].FindElements(By.XPath("td[@ng-repeat='column in columns']"));
                    string nameColumn = columns[0].Text;
                    string userColumn = columns[2].Text;
                    string customerColumn = columns[5].Text;
                    string cellPhoneColumn = columns[7].Text;

                    if (nameColumn.ToLower() == firstName.ToLower() && 
                        userName.ToLower() == userColumn.ToLower() &&  
                        customerColumn.ToLower() == role.ToLower() &&
                        cellPhone.ToLower() == cellPhoneColumn.ToLower())
                    {
                        // Get a collection of delete buttons and click the correct one
                        ReadOnlyCollection<IWebElement> deleteBttns = _Driver.FindElements(By.XPath("//button[@ng-click='delUser()']"));
                        IWebElement delBttn = deleteBttns[i];
                        delBttn.Click();

                        // Confirm the deletion
                        IWebElement confirmDeleteBttn = _Driver.FindElement(By.XPath("//button[@class='btn ng-scope ng-binding btn-primary']"));
                        confirmDeleteBttn.Click();

                        break;
                    }
                    else
                        continue;
                }
            }
            catch(Exception error)
            {
                Console.WriteLine(error.Message);
                return false;
            }

            return success;
        }
        #endregion
        #region EnterDataIntoTextField
        public static bool EnterDataIntoTextField(string elementName, string dataToEnter)
        {
            bool success = false;

            try
            {
                IWebElement element = _Driver.FindElement(By.Name(elementName));
                element.Clear();
                element.SendKeys(dataToEnter);
                success = true;

                //string cellphoneContent = cellPhoneField.Text;
                //if (cellphoneContent != "")
                //    success = true;
                //else
                //    success = false;
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                return false;
            }

            return success;
        }
        #endregion
        #region IsElementPresent
        public static bool IsElementPresent(By by)
        {
            try
            {
                return _Driver.FindElement(by).Displayed;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #region SelectCustomerRadioBttn
        public static bool SelectCustomerRadioBttn(string elementName, string itemToSelect)
        {
            bool success = false;

            try
            {
                ReadOnlyCollection<IWebElement> customerFields = _Driver.FindElements(By.Name(elementName));
                IWebElement customer = _Driver.FindElement(By.XPath(itemToSelect));
                customer.Click();
                bool customerState = customer.Selected;
                if (customerState)
                    success = true;
                else
                    success = false;
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                return false;
            }

            return success;
        }
        #endregion
        #region SelectRole
        public static bool SelectRole(string element, string roleToSelect)
        {
            bool success = false;

            try
            {
                IWebElement dropdownElement = _Driver.FindElement(By.Name(element));
                SelectElement selectDropdownElement = new SelectElement(_Driver.FindElement(By.Name(element)));
                selectDropdownElement.SelectByText(roleToSelect);
                string selectedItem = selectDropdownElement.SelectedOption.Text;

                if (selectedItem == roleToSelect)
                    success = true;
                else
                    success = false;
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                return false;
            }

            return success;
        }
        #endregion
        #region WaitForElement
        public static bool WaitForElement(string element, _By by, int timeoutHalfSeconds)
        {
            for (int i = 0; i < timeoutHalfSeconds; i++)
            {
                switch (by)
                {
                    case _By.className:
                        if (IsElementPresent(By.ClassName(element)))
                        {
                            i = timeoutHalfSeconds;
                            return true;
                        }
                        break;
                    case _By.cssSelector:
                        if (IsElementPresent(By.CssSelector(element)))
                        {
                            i = timeoutHalfSeconds;
                            return true;
                        }
                        break;
                    case _By.id:
                        if (IsElementPresent(By.Id(element)))
                        {
                            i = timeoutHalfSeconds;
                            return true;
                        }
                        break;
                    case _By.linkText:
                        if (IsElementPresent(By.LinkText(element)))
                        {
                            i = timeoutHalfSeconds;
                            return true;
                        }
                        break;
                    case _By.partialLinkText:
                        if (IsElementPresent(By.PartialLinkText(element)))
                        {
                            i = timeoutHalfSeconds;
                            return true;
                        }
                        break;
                    case _By.tagName:
                        if (IsElementPresent(By.TagName(element)))
                        {
                            i = timeoutHalfSeconds;
                            return true;
                        }
                        break;
                    case _By.xPath:
                        if (IsElementPresent(By.XPath(element)))
                        {
                            i = timeoutHalfSeconds;
                            return true;
                        }
                        break;
                    default:
                        if (IsElementPresent(By.Name(element)))
                        {
                            i = timeoutHalfSeconds;
                            return true;
                        }
                        break;
                }

                System.Threading.Thread.Sleep(500);
            }

            return false;
        }
        #endregion
        #region VerifyRowAddedToTable
        public static bool VerifyRowAddedToTable(string firstName, string role)
        {
            bool success = false;

            try
            {
                ReadOnlyCollection<IWebElement> rows = _Driver.FindElements(By.XPath("//tr[@ng-repeat='dataRow in displayedCollection']"));
                
                for(int i = 0; i < rows.Count; i++)
                {
                    ReadOnlyCollection<IWebElement> columns = rows[i].FindElements(By.XPath("td[@ng-repeat='column in columns']"));

                    string nameColumn = columns[0].Text;
                    string customerColumn = columns[5].Text;

                    if (nameColumn.ToLower() == firstName.ToLower() && customerColumn.ToLower() == role.ToLower())
                    {
                        success = true;
                        break;
                    }
                    else
                        continue;
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                return false;
            }

            return success;
        }
        #endregion
        #region VerifyDeletedFromTable
        public static bool VerifyDeletedFromTable(string firstName, string role)
        {
            bool success = false;
            bool userNotFound = false;

            try
            {
                ReadOnlyCollection<IWebElement> rows = _Driver.FindElements(By.XPath("//tr[@ng-repeat='dataRow in displayedCollection']"));

                for(int i = 0; i < rows.Count; i++)
                {
                    ReadOnlyCollection<IWebElement> columns = rows[i].FindElements(By.XPath("td[@ng-repeat='column in columns']"));
                    userNotFound = false;

                    string nameColumn = columns[0].Text;
                    string customerColumn = columns[5].Text;

                    if (nameColumn.ToLower() == firstName.ToLower() && customerColumn.ToLower() == role.ToLower())
                    {
                        success = false;
                        break;
                    }
                    else
                        userNotFound = true;
                }

                success = userNotFound;
            }
            catch(Exception error)
            {
                Console.WriteLine(error.Message);
                return false;
            }

            return success;
        }
        #endregion
    }
}
