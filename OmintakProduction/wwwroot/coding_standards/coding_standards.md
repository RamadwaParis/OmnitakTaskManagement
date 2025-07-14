# 📄 Coding Standards

## 👨‍💻 Prepared by DreamSpace

### ✅ Naming conventions
- **Classes & Models:** `PascalCase`
  - Example: `TaskController`, `UserService`
	
- **Methods:** `PascalCase`
  - Example: `GetUserById()`
	
- **Variables & Fields:** `camelCase`
  - Example: `userId`, `taskList`
	
- **Constants:** `ALL_CAPS`
  - Example: `MAX_TASKS`, `DEFAULT_ROLE`

---

### ✅ Formatting & Style
-

---

### ✅ Best practices
- Validate all user inputs on both client and server side.


---

## 🌳 Branch naming conventions & general structure

To ensure clarity and consistency when collaborating, we follow this branch naming pattern:

---

### 🔷 Common prefixes
| Prefix         | When to use |
|----------------|-------------|
| `feature/`     | New features or enhancements |
| `bugfix/`      | Fixing a bug |
| `docs/`        | Documentation updates |
| `refactor/`    | Code cleanup or restructuring |

---

### 🔷 Examples
| Purpose                    | Branch name |
|----------------------------|-------------|
| Add login functionality    | `feature/login-page` |
| Fix email valdation error  | `bugfix/email-validation` |
| Update README file         | `docs/update-readme` |
| Refactor dashboard code    | `refactor/dashboard-layout` |
| Add project models from ERD| `feature/add-erd-models` |

---

### 📝 Tips for Branch Names
✅ Be descriptive but concise.  
✅ Use hyphens `-` instead of spaces or underscores.  
✅ Avoid very long branch names.  
✅ Always use lowercase letters.

### ✅ Commenting Guidelines
- Use clear, concise comments to explain complex logic or decisions.

### ✅ Code Review Process
- All code must be reviewed by all team members before merging.
