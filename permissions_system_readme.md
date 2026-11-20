# 🔐 C# Bitwise Permissions Management System

A lightweight, high-performance, and extensible **Role/Feature Permissions Management System** built in C#. It combines **Bitwise Operations** for ultra-fast permission evaluation with **Reflection** for dynamic feature discovery and runtime extensibility.

---

## ✨ Key Features

- **Bitwise Access Control:** Uses powers of 2 ($2^n$) to store multiple user permissions inside a single `int` value, enabling $O(1)$ permission checks.
- **Dynamic Feature Discovery (Reflection):** Automatically loads base features and their bitmask values from `clsFeatures` at initialization.
- **Runtime CRUD Operations:** Dynamically add new permissions (auto-calculating the next power of 2), update existing permission names, or drop obsolete ones.
- **Full Admin & Empty States:** Built-in support for `All (-1)` and `None (0)` states, including smart revocation when downgrading a user from full admin privileges (`-1`) to granular permissions.
- **Human-Readable Inspection:** Easily decode and print a user's composite integer permission mask into a clean list of granted feature names.

---

## 🛠️ How It Works

Each feature in the system is represented by a unique power of 2:

| Feature | Integer Value | Binary Representation |
| :--- | :---: | :---: |
| `None` | `0` | `0000 0000` |
| `Customers` | `1` | `0000 0001` |
| `Products` | `2` | `0000 0010` |
| `Insert` | `4` | `0000 0100` |
| `Delete` | `8` | `0000 1000` |
| `Update` | `16` | `0001 0000` |
| `Read` | `32` | `0010 0000` |
| `Write` | `64` | `0100 0000` |
| `DeleteEmp` | `128` | `1000 0000` |
| `All` | `-1` | `1111 1111` (All bits set) |

### Core Bitwise Operations Used
- **Grant Permission (`|`):** `userPermissions | permissionToGrant`
- **Revoke Permission (`& ~`):** `userPermissions & ~permissionToRevoke`
- **Verify Permission (`&`):** `(userPermissions & requiredPermission) == requiredPermission`
- **Validate Power of 2:** `(x > 0) && ((x & (x - 1)) == 0)`

---

## 🚀 Usage Example

### 1. Define Base Features
```csharp
public class clsFeatures
{
    public int All = -1;
    public int None = 0;
    public int Customers = 1;
    public int Products = 2;
    public int Insert = 4;
    public int Delete = 8;
    public int Update = 16;
    public int Read = 32;
    public int Write = 64;
    public int DeleteEmp = 128;
}
```

### 2. Initialize & Manage User Permissions
```csharp
// Initialize the permissions engine
Permissions p = new Permissions(new clsFeatures());

// Display all registered system permissions
p.Print();

// Assign initial permissions (Customers + Products + Update)
int userPermissions = p.Features.Customers | p.Features.Products | p.Features.Update;
p.PrintUserPermissions(userPermissions);
// Output: User Permissions: Customers, Products, Update

// Grant additional permissions
userPermissions = p.GrantPermission(userPermissions, p.Features.Read | p.Features.DeleteEmp);
p.PrintUserPermissions(userPermissions);
// Output: User Permissions: Customers, Products, Update, Read, DeleteEmp

// Revoke specific permissions
userPermissions = p.RevokePermission(userPermissions, p.Features.Update | p.Features.DeleteEmp);
p.PrintUserPermissions(userPermissions);
// Output: User Permissions: Customers, Products, Read

// Check if user has access to a specific feature
bool canRead = p.HasPermission(userPermissions, p.Features.Read); // true
bool canDelete = p.HasPermission(userPermissions, p.Features.Delete); // false
```

### 3. Manage System Permissions Dynamically
```csharp
// Add a new permission at runtime (automatically assigned 256)
p.AddNewPermission("ExportReports");

// Rename an existing permission
p.Update(256, "ExportFinancialReports");

// Remove a permission from the registry
p.DropPermission(256);
```

---

## 📋 API Reference

| Method | Description |
| :--- | :--- |
| `Print()` | Prints all available permissions in the system sorted by their bitmask key. |
| `AddNewPermission(string name)` | Registers a new permission and automatically assigns the next available power of 2. |
| `Update(int num, string name)` | Updates the display name of an existing permission key. |
| `DropPermission(int num)` | Removes a permission from the system dictionary. |
| `GrantPermission(int userPerm, int toGrant)` | Combines new permission(s) into the user's current permission mask. |
| `RevokePermission(int userPerm, int toRevoke)` | Strips specific permission(s) from the user's mask (handles `All = -1` downgrade automatically). |
| `HasPermission(int userPerm, int required)` | Returns `true` if the user holds the required permission(s) or has `All (-1)`. |
| `PrintUserPermissions(int userPerm)` | Decodes the user's integer mask and prints all granted feature names. |

---

## ⚠️ Technical Notes
- **Capacity:** Using a 32-bit signed `int` supports up to **31 distinct permissions** ($2^0$ through $2^{30}$). If your application requires more than 31 features, change the underlying key and mask types from `int` to `long` (supports up to 63 permissions).
- **Database Storage:** Only a single `INT` column is required in your `Users` or `Roles` table to store any combination of permissions.