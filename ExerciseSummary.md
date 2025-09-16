## Rules

- [x] A Person is uniquely identified by their Name.
- [x] A Person who has not had an astronaut assignment will not have Astronaut records.
	- `GET /AstronautDuty/{name}` does return a 200 for such a person, but with no current duty details or duty records.
- [x] A Person will only ever hold one current Astronaut Duty Title, Start Date, and Rank at a time.
- [x] A Person's Current Duty will not have a Duty End Date.
- [x] A Person's Previous Duty End Date is set to the day before the New Astronaut Duty Start Date when a new Astronaut Duty is received for a Person.
- [x] A Person is classified as 'Retired' when a Duty Title is 'RETIRED'.
- [x] A Person's Career End Date is one day before the Retired Duty Start Date.

## Requirements

- [x] Retrieve a person by name.
- [x] Retrieve all people.
- [x] Add/update a person by name.
	- Added `PUT /Person/{name}` endpoint.	
- [x] Retrieve Astronaut Duty by name.
- [x] Add an Astronaut Duty.

## Tasks

### Examine the code ✅

Along the way I resolved numerous static analysis warnings and messages such as possible-null-reference, cancellation token propagation, etc.

### Find and resolve any flaws
- `obj` folder was tracked in source control. Deleted and applied basic .NET gitignore rules.
- Generated EF migration to fix `AstronautDetail.CareerStartDate` nullability mismatch between model and database.
- `POST /AstronautDuty`'s logic for preventing duplicate was not checking astronaut name, so it would have prevented any two astronauts from having the same title and start date. Fixed by checking only duties for the relevant astronaut.

### Identify design patterns and follow or change them
- Replaced all hand-written SQL with EF-generated queries. Removed then unused Dapper dependency.
- The original approach to duplicate record prevention (using Mediatr prepocessors to check for existing records) presented a possible race condition. I resolved that by moving the check into the command handlers and wrapping everything in database transactions.
   - Created a simple unit-of-work style utility for performing operations in a transaction.
- Got rid of the `BaseResponse` approach to indicating success or failure of a request. Instead, I'm using an `IExceptionFilter` to handle expected and unexpected exceptions and interpret them as `ProblemDetails`. This also allowed me to get rid of a few cases of one-off exception handling in controller actions.
- Introduced `AsNoTracking` in a few additional readonly operations to improve performance.
- Added basic logging of unhandled exceptions.
- Created a home for shared constant values (but only ended up finding one: `"RETIRED"`).
- Introduced `ProducesResponseType` attributes for all actions, resulting in a more detailed Open API specification, which allows client-generation tools to produce a more useful client.

### Generate the database ✅

### Enforce the rules ✅

### Improve defensive coding
- Introduced more intentional constructors and factory methods to many classes, making it easier to create valid instances.

### Add unit tests
- Added database integration tests for all actions using `WebApplicationFactory` and a SQLite in-memory database spun up fresh for each test to ensure consistency.

### Implement process logging
- Used a pretty simple combination of a middleware and a scoped `ProcessLoggingService` to capture key details of all write operations and record them to the database. [More info](https://github.com/JackOlsen/bamtech-exercise-jo2025/pull/12).

### Did not do (due to time constraints) ⌛
- Add comprehensive tests covering basic validation of all requests, i.e.: required fields, types, etc. The API is enforcing this, I just don't have tests to prove it.
- Implement a user interface. I scaffolded an Angular app but then only had about an hour left and figured there was no way I was going to put anything worthwhile together that quickly.
