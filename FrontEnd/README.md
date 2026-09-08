# BridgeStartup

Angular frontend connected to the ASP.NET Core API in `../Backend`.

## Run locally

Start the backend:

    dotnet run --project ../Backend/Backend/Backend.csproj --launch-profile https

Start the frontend in another terminal:

    npm install
    npm start

Open http://localhost:4200. Sign in at `/login` with your existing active admin account, then open `/admin/users` or `/admin/posts`.

The backend uses its existing SQL Server connection, schema, and role permissions. Public registration also requires SMTP settings and email activation. The development proxy forwards `/api` to `https://localhost:7086`; change `proxy.conf.json` if your API runs elsewhere. Production needs an `/api` reverse proxy and an `index.html` fallback for frontend routes.

## API connections

| Method | Route | Purpose |
| --- | --- | --- |
| POST | `/api/Auth/login` | Email/password login |
| POST | `/api/Auth/register` | Registration with email activation |
| GET | `/api/Posts?Title=...&SortBy=title&SortOrder=asc&Page=1` | Title search, server sorting, five posts per page |
| GET | `/api/Posts/{id}` | Post details with nested author and badge names |
| POST multipart | `/api/Posts/apply` | Apply to the selected post with a CV |
| GET | `/api/admin/roles` | Admin role choices |
| GET, POST | `/api/admin/users` | Complete admin user list and user creation |
| GET | `/api/admin/users/{id}` | Current role and activation state before an edit |
| PATCH | `/api/Users/{id}` | Username, email, and optional password updates |
| PUT | `/api/admin/users/{id}` | Edits that also change role or activation |
| DELETE | `/api/Users/{id}` | User deletion |
| GET | `/api/admin/posts` | Founder IDs needed by the admin editor |
| POST | `/api/admin/posts` | Create posts with badge names |
| PUT | `/api/admin/posts/{id}` | Full post edit including founder and clearing badges/contacts |
| DELETE | `/api/Posts/{id}` | Permanent post deletion |

Both public and admin post lists use the backend's sorting and five-record pagination. Admin search filters the displayed page. The API returns an array without a total count, so a full last page can offer Next followed by an empty page; Previous remains available.

Some admin routes remain necessary with the current resource contracts. `/Users` responses omit `roleId`/`isActive` and include password hashes; the frontend reads the safe, complete admin DTO instead. `UpdateUserDTO` supports profile/password changes but not role/activation. `POST /Posts` requires numeric badge IDs, and the badge catalog endpoint is pending. Resource post PATCH cannot change the founder, create badge names, or clear all badges/null contacts. The existing admin editor routes preserve those operations. No `POST /Users` or badge catalog endpoint was added.

## User updates and applications

`UsersController.PATCH` now assigns the route ID to `UpdateUserDTO` and invokes the existing `IUpdateUserCommand` through `UseCaseHandler`. `EfUpdateUserCommand` remains in the Implementation layer and is registered in dependency injection. An omitted or blank password preserves the existing hash. The calling role needs the existing `update-user` permission.

Applications submit `PostId` and `userFile` as `FormData`. The server gets the applicant ID from the validated JWT. The browser supplies the multipart Content-Type boundary automatically. A guest is sent to login and returned to the selected post.

The upload command now implements an awaitable Application-layer command interface. `UseCaseHandler` checks the existing `apply-to-post-locally` permission; the Implementation layer validates and stores the CV and saves a `PostApplication` with the user/post IDs. The controller returns 204 after both operations finish. Duplicate applications return 409, invalid files return 422, and missing form fields return 400. Application detail DTO properties also support route binding and JSON serialization; a missing application returns 404.

CVs must be non-empty PDF, DOC, or DOCX files up to 5 MB, with matching signatures. Generated filenames are stored under `Backend/Backend/App_Data/Applications`, outside public `wwwroot`. Override `Uploads:ApplicationsPath` for another private directory. Failed saves remove the newly written file. No download endpoint or database migration was added.

Logout clears the browser session, current user, and expiry timer, and returns to `/login`. It also synchronizes across tabs. The backend has no logout/revocation endpoint; its JWTs expire normally. Authenticated 401 responses and expired sessions sign the browser out.

## Validation

    npm test
    npm run build
    dotnet run --project tests/backend/Backend.IntegrationTests.csproj
    npm run test:browser

Frontend tests cover authentication, guards, error handling, CRUD routing/state, sorting parameters, and multipart requests. Backend integration tests use an isolated SQLite database and upload directory, covering the current API contracts, PATCH persistence/password changes, application storage, validation, duplicates, and retrieval. They do not modify SQL Server data.

Browser checks run headless Edge against local API fixtures after a production build. They cover user/post CRUD, pagination, sorting, return-to-post login, CV submission, duplicate feedback, logout, and mobile layout. Set `BROWSER_PATH` to another Chromium-compatible executable if needed. Screenshots are written under `tmp/`.

Production output: `dist/FrontEnd/browser`.
