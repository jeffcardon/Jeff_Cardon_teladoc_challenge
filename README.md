# Jeff_Cardon_teladoc_challenge

Teladoc Challenge

This app is used to add/remove a user's data from the following web site: http://www.way2automation.com/angularjs-protractor/webtables/. 
It will add a new user to the table, or delete an existing user from the table. It takes the following paramaters:

----------------------------------------------------------------------------------------------------------------------
| Item         | Parameter                     | Description                                             | Required  |
----------------------------------------------------------------------------------------------------------------------
| Action       | Add / Delete                  | Adds or deletes a user from the table                   | Yes       |
----------------------------------------------------------------------------------------------------------------------
| First name   | [name]                        | Specifies the name of the individual to add or delete   | Yes       |
----------------------------------------------------------------------------------------------------------------------
| Username     | [username]                    | Specifies the username of the individual                | Yes       |
----------------------------------------------------------------------------------------------------------------------
| Role         | Customer / SalesTeam / Admin  | The roll of the individual                              | Yes       |
----------------------------------------------------------------------------------------------------------------------
| Cell phone   | [cell phone]                  | The cell phone of the individual                        | Yes       |
----------------------------------------------------------------------------------------------------------------------
| Last name    | [name]                        | The last name of the individual                         | No        |
----------------------------------------------------------------------------------------------------------------------
| Password     | [password]                    | The password of the individual                          | No        |
----------------------------------------------------------------------------------------------------------------------
| Customer     | CompanyAAA / CompanyBBB       | The name of the customer                                | No        |
----------------------------------------------------------------------------------------------------------------------
| Email        | [email]                       | The email of the individual                             | No        |
----------------------------------------------------------------------------------------------------------------------

Minimum required syntax:
 Teladoc.exe [Action] [First name] [Username] [Role] [Cell phone]

Maximum syntax:
 Teladoc.exe [Action] [First name] [Username] [Role] [Cell phone] [Last name] [Password] [Customer] [Email]

Examples:
To add a customer named John Doe, with all the user's details, to the table:
 Teladoc.exe Add John jdoe Customer 9072271698 Doe p@s$w0rD CompanyBBB jdoe@msn.com

To delete a customer from the table, with all the user's details:
 Teladoc.exe Delete Mark novak Customer, 9072256710 Novak p@s$w0rD CompanyAAA asa@asd.cz
 
