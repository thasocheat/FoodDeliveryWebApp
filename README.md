# FoodDeliveryWebApp

## 🏃 Getting Started

1. Go into directory where you plan on keeping project and run.

```bash
  git fork https://github.com/thasocheat/FoodDeliveryWebApp
  
```

- Or
  
```bash
  
  git clone https://github.com/thasocheat/FoodDeliveryWebApp
```

2. Create a local database.

Note:

```bash
 Data Source=DESKTOP-0CLKE34;Initial Catalog=FoodDeliveryWebApps;User ID=sa;Password=123;
```

- Data `Source=DESKTOP-0CLKE34;` is your computer name.
- In this project the database name is `FoodDeliveryWebApps`.
- `User ID` your database username.
- And the password.

3. Add connection string to app `appsettings.json`. It will look something like this:
```bash
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=DESKTOP-0CLKE34;Initial Catalog=FoodDeliveryWebApp;User ID=sa;Password=123;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi   Subnet Failover=False"
},
```

4. Go to Package Manager Console and type:

   ![image](https://github.com/thasocheat/FoodDeliveryWebApp/assets/96945084/bb815604-161a-450c-af59-ab91c094bccb)


- To create the migration folder and files
   ```bash
    Add-Migration InitialCreate 
   ```

Note:

- When clone the project it already have the Migration folder, so you no need to use the command above.
- You only just use the command below to update the database and it will migrate the tables.  

 ```bash
    Update-Database 
   ```
  - Go to your project directory or `Window PowerShell` just type the command below
  - This command is to seed data into table for testing this project.

  ```bash
    dotnet run seeddata 
   ```
  ![image](https://github.com/thasocheat/FoodDeliveryWebApp/assets/96945084/63c09d8a-7413-4e72-99c7-3c2d7b0c864b)


  
5. This project has three different users:
- Admin: admin@gmail.com
- Staff: staff@worker.com
- User: user@user.com
- Same Password: `Coding@1234?`
