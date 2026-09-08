# BridgeStartup

Angular frontend connected to the ASP.NET Core API in ../Backend. JSON Server is no longer used.

## Run locally

Start the backend in one terminal:

    dotnet run --project ../Backend/Backend/Backend.csproj --launch-profile https

Start the frontend in another:

    npm install
    npm start

Open http://localhost:4200. Sign in at /login with your existing active admin account, then open /admin/users or /admin/posts.

The backend requires its existing SQL Server connection, schema, and roles. Public registration also requires working SMTP settings and email activation. Admin-created accounts can be activated directly in the admin form.

proxy.conf.json forwards /api requests to https://localhost:7086, matching the backend's HTTPS launch profile. Change that target if your API runs elsewhere. The proxy accepts the local development certificate. For production, serve the built frontend with an /api reverse proxy to ASP.NET and an index.html fallback for frontend routes.

## Features

- Public post list, title search, backend title sorting (A to Z / Z to A), and post details with embedded author/badge data.
- CV applications from the selected post, with sign-in return navigation, file validation, and duplicate-application feedback.
- Login, registration, saved JWT session, expiry handling, and logout across tabs.
- Admin user creation, listing, editing, role/activation management, and deletion.
- Admin post creation, listing, editing, founder assignment, badge editing, and deletion.
- Validation messages, loading states, retries, search, and delete confirmations.

Admin routes require a validated JWT and an active account whose current database role is admin. Disabled/deleted users and demoted admins lose access even when their JWT has not expired. Passwords are BCrypt-hashed and never returned in user responses. Leaving a password blank during an edit preserves it.

Deleting a user or post sets the existing DeletedAt fields. Deleted users lose access, and their posts/applications are hidden. Deleted emails remain reserved by the database's unique email index. Admins cannot delete themselves, deactivate themselves, or change their own role. Saving other changes to your own account signs you out so you can log in with the updated details.

Logout removes the browser session, clears the current user, stops the expiry timer, and returns to /login. The backend uses stateless JWTs and has no logout/revocation endpoint; a copied token remains valid until its expiry unless the account is disabled/deleted. Expired sessions and authenticated 401 responses also sign the browser out.

## API routes used by the frontend

| Method | Route | Purpose |
| --- | --- | --- |
| POST | /api/Auth/login | Email/password login; returns user and token |
| POST | /api/Auth/register | Register; activation email required |
| GET | /api/Posts?Title=...&SortBy=title&SortOrder=asc | Public posts, title filter, and backend sorting (`asc` or `desc`) |
| GET | /api/Posts/{id} | Public details including author and badges |
| GET | /api/admin/roles | Role choices |
| GET, POST | /api/Users | Admin list/create users |
| GET, PUT, DELETE | /api/Users/{id} | Admin read/update/delete user |
| POST (JSON) | /api/Posts | Admin create post |
| PATCH, DELETE | /api/Posts/{id} | Admin update/delete post |
| POST (multipart) | /api/Posts | Apply to the selected post with a CV |

UsersController owns user CRUD, and PostsController owns post CRUD and applications. Existing user/post queries and the post update command are reused. AdminController only supplies role choices; the old /api/admin/users and /api/admin/posts API routes have been removed. The frontend admin page URLs remain /admin/users and /admin/posts.

POST /api/Posts distinguishes JSON post creation from multipart applications by Content-Type. Applications submit PostId and userFile. The server gets the applicant's identity from the validated JWT, ignoring any submitted UserId. /api/Posts/apply is also accepted as a multipart alias. Do not manually set Content-Type when sending FormData: the browser supplies the multipart boundary.

Applications accept non-empty PDF, DOC, or DOCX files up to 5 MB. The server validates the file signature and writes a PostApplication record after the upload finishes. Duplicate applications return HTTP 409. Uploaded CVs use generated filenames under Backend/Backend/App_Data/Applications, outside the publicly served wwwroot folder. Override Uploads:ApplicationsPath to use another private directory; the backend process needs write access to it. An application requires the existing apply-to-post-locally permission on the user's role.

GET /api/Users returns all active (not deleted) records when Page is omitted, so admin lists and founder selectors include every account. Supplying Page retains the existing five-record pagination. Post PATCH preserves omitted fields, accepts null to clear email/phone, and accepts [] to remove all badges.

## Validation

    npm test
    npm run build
    dotnet run --project tests/backend/Backend.IntegrationTests.csproj
    npm run test:browser

Frontend tests exercise sessions, guards, HTTP errors, CRUD routing/state, sorting parameters, and multipart payloads. Backend integration checks use an isolated in-memory SQLite database and test authentication, authorization, CRUD, partial updates, sorting, uploads, duplicates, and soft deletion. They do not modify your SQL Server data. Test CVs are stored under the ignored test build directory.

Browser checks require a production build and run headless Edge against isolated API fixtures. On a different machine, set BROWSER_PATH to a Chromium/Chrome/Edge executable. The checks cover login, user/post CRUD, sorting, CV submission, duplicate feedback, logout, and the mobile layout. Screenshots are written under tmp/.

Production output: dist/FrontEnd/browser.
