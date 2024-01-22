# FoodDeliveryWebApp

## 🏃 Getting Started

1. Go into directory where you plan on keeping project and run.

```bash
  git fork https://github.com/thasocheat/FoodDeliveryWebApp
  or git clone [https://github.com/thasocheat/GroopRunWebApp.git](https://github.com/thasocheat/FoodDeliveryWebApp)
```

2. Create a local database.
```bash
 Data Source=DESKTOP-0CLKE34;Initial Catalog=FoodDeliveryWebApps;User ID=sa;Password=123;
```
- Data Source is your computer name.
- In this project the database name is FoodDeliveryWebApps.
- User ID your database user name.
- And password.

3. Add connection string to app appsettings.json. It will look something like this:
```bash
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=DESKTOP-0CLKE34;Initial Catalog=FoodDeliveryWebApp;User ID=sa;Password=123;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi   Subnet Failover=False"
},
```

4. Go to Window PowerShell and type:

- To create the migration file
   ```bash
    Add-Migration InitialCreate 
   ```
- To run and migrate the tables into the database, just go to your project directory
 ```bash
    Update-Database 
   ```
  
  ![image](https://github.com/thasocheat/FoodDeliveryWebApp/assets/96945084/63c09d8a-7413-4e72-99c7-3c2d7b0c864b)

- This command is to seed data into table for testing this project
  ```bash
    dotnet run seeddata 
   ```
  
5. This project has three different users:
- Admin: admin@gmail.com
- Staff: staff@worker.com
- User: user@user.com
- Password is the same: Coding@1234?
