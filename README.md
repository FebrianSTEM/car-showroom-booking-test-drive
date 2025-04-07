# Deskripsi Project
Mini project ini saya buat bertujuan untuk Calon Customer dapat melakukan Booking test drive secara online dan menentukan kendaran serta waktu yang mereka inginkan untuk melakukan test drive.

# Services
Mini Project ini terdiri dari 3 services
 1. user-service : bertujuan untuk registrasi serta authentikasi yang digunakan di service lain, dibuat menggunakan
    - dotnet 8.0
    - entity framework core in memory database.
 2. car-booking-service : bertujuan untuk handling data mobil serta booking mobil, dibuat menggunakan
    - dotnet 8.0
    - entity framework core in memory database.
 3. car-booking-fe-service : digunakan sebagai tampilan depan (front end) dari service yang berinteraksi langsung dengan user, service ini dibuat menggunakan react.


# Schema Database

Walaupun pada mini project ini saya menggunakan in memory database, artinya aplikasi berjalan dengan tidak menyimpan langsung ke database asli. Namun apabia dibutuhkan berikut ini saya lampirkan database schema yang saya gunakan sebagai entity model.


```sql
-- TABEL Roles
CREATE TABLE Roles (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);

-- Index untuk Role Name (jika sering dicari berdasarkan nama)
CREATE INDEX IX_Roles_Name ON Roles(Name);

-- TABEL Users
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(50),
    PasswordHash NVARCHAR(MAX) NOT NULL,
    EmailConfirmed BIT NOT NULL DEFAULT 0,
    PhoneNumberConfirmed BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    LastLoginAt DATETIME2 NULL
);

-- Index unik untuk Email
CREATE UNIQUE INDEX IX_Users_Email ON Users(Email);

-- Index untuk PhoneNumber (jika sering digunakan dalam pencarian)
CREATE INDEX IX_Users_PhoneNumber ON Users(PhoneNumber);

-- TABEL UserRoles (Many-to-Many)
CREATE TABLE UserRoles (
    UserId UNIQUEIDENTIFIER NOT NULL,
    RoleId UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE
);

-- Index untuk mempercepat JOIN dan pencarian
CREATE INDEX IX_UserRoles_UserId ON UserRoles(UserId);
CREATE INDEX IX_UserRoles_RoleId ON UserRoles(RoleId);
```

```sql
-- SCHEMA: Master
CREATE SCHEMA Master;
GO

-- TABLE: Master.CarModels
CREATE TABLE Master.CarModels (
    CarId INT IDENTITY(1,1) PRIMARY KEY,
    Brand NVARCHAR(50) NOT NULL,
    Model NVARCHAR(100) NOT NULL,
    Year INT NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL,
    Description NVARCHAR(500),
    IsAvailableForTestDrive BIT NOT NULL,
    CreatedBy NVARCHAR(50) NOT NULL DEFAULT 'system',
    CreatedAt DATETIME2 NOT NULL,
    UpdatedBy NVARCHAR(50) NOT NULL DEFAULT 'system',
    UpdatedAt DATETIME2 NOT NULL
);

-- Indexing: Master.CarModels
CREATE INDEX IX_CarModels_Brand ON Master.CarModels(Brand);
CREATE INDEX IX_CarModels_Model ON Master.CarModels(Model);
CREATE INDEX IX_CarModels_Year ON Master.CarModels(Year);

--------------------------------------------------------------------------------

-- SCHEMA: BookingTrx
CREATE SCHEMA BookingTrx;
GO

-- TABLE: BookingTrx.TestDrive
CREATE TABLE BookingTrx.TestDrive (
    BookingId INT IDENTITY(1,1) PRIMARY KEY,
    CarId INT NOT NULL,
    StartBookingDate DATETIME2 NOT NULL,
    EndBookingDate DATETIME2 NOT NULL,
    CreatedBy NVARCHAR(50) NOT NULL DEFAULT 'system',
    CreatedAt DATETIME2 NOT NULL,
    UpdatedBy NVARCHAR(50) NOT NULL DEFAULT 'system',
    UpdatedAt DATETIME2 NOT NULL,
    CONSTRAINT FK_TestDrive_CarModel FOREIGN KEY (CarId)
        REFERENCES Master.CarModels(CarId)
        ON DELETE NO ACTION -- Restrict behavior
);

-- Indexing: BookingTrx.TestDrive
CREATE INDEX IX_TestDrive_CarId ON BookingTrx.TestDrive(CarId);
CREATE INDEX IX_TestDrive_StartBookingDate ON BookingTrx.TestDrive(StartBookingDate);
CREATE INDEX IX_TestDrive_EndBookingDate ON BookingTrx.TestDrive(EndBookingDate);
```

# Seeded Account

Dalam rangka mempermudah pengetesan, pada saat service berjalan akan otomatis menambahkan 2 akun sebagai berikut:
 1. - email : admin@mail.com
    - phone : 080989999
    - role  : Admin
    - pass  : Admin123
    
 2. - email : user@mail.com
    - phone : 085752082822
    - role  : User
    - pass  : User123


# How to Run
Untuk bagaimana cara setup dan running services bisa mengacu pada penjelasan berikut:
1. user-service :
   - [Setup](https://github.com/FebrianSTEM/car-showroom-booking-test-drive/blob/main/user-service/SETUP.md)
   - [Documentation](https://github.com/FebrianSTEM/car-showroom-booking-test-drive/blob/main/user-service/DOCUMENTATION.md)
2. car-booking-service : 
   - [Setup](https://github.com/FebrianSTEM/car-showroom-booking-test-drive/blob/main/Car-Booking-Service/SETUP.md)
   - [Documentation](https://github.com/FebrianSTEM/car-showroom-booking-test-drive/blob/main/Car-Booking-Service/DOCUMENTATION.md)
3. car-booking-fe-service : 
   - Pastikan Backend Service telah berjalan
   - change directory ke car-booking-fe-service/car-booking
   - kemudian running dengan mengetikkan command berikut pada terminal : npm run dev
   - Apabila sudah muncul tampilan seperti dibawah ini maka anda bisa mengakses UI di alamat local : http://localhost:5173/ (address akan bervariasi bergantung dengan ketersedian port)
    ``` bash
     VITE v6.2.5  ready in 192 ms
      ➜  Local:   http://localhost:5173/
      ➜  Network: use --host to expose
      ➜  press h + enter to show help
    ```
    - Apabila ada penyesuaian pada port backend maka harap mengubah pada 
      car-booking-fe-service/car-booking/vite.config.ts sperti berikut
    ``` json

        import { defineConfig } from 'vite'
        import react from '@vitejs/plugin-react-swc'
        
        // https://vite.dev/config/
        export default defineConfig({
          plugins: [react()],
            server: {
              proxy: {
                '/auth': {
                  target: 'https://localhost:7125', //port harap disesuaikan
                  changeOrigin: true,
                  rewrite: (path) => path.replace(/^\/auth/, '/api'),
                  secure: false,
                },
                '/carBooking': {
                  target: 'https://localhost:7126', //port harap disesuaikan
                  changeOrigin: true,
                  rewrite: (path) => path.replace(/^\/carBooking/, '/api'),
                  secure: false,
                },
              },
            }
        })
    ```