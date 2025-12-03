## Beispiel: Nutzer-Registrierung

### Anfrage (POST /api/auth/register)
```
{
  "email": "superuser@test.local",
  "password": "Super123!",
  "firstName": "Super",
  "lastName": "User",
  "role": "SuperUser"
}
```
**Antwort**
Status: `200 OK` (User created successfully)


## Fehlerfall: Registrierung