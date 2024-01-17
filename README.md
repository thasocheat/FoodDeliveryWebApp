# FoodDeliveryWebApp

## 🏃 Getting Started

1. Go into directory where you plan on keeping project and run.

```bash
  git fork https://github.com/thasocheat/FoodDeliveryWebApp
  or git clone [https://github.com/thasocheat/GroopRunWebApp.git](https://github.com/thasocheat/FoodDeliveryWebApp)
```

2. Create a local database.


3. Add connection string to app settings.json. It will look something like this:
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
- To run and migrate the tables into the database
