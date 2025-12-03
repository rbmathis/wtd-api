# 🎯 WillTheyDie API - Implementation Plan for Frontend Integration

**Project**: WillTheyDie API  
**Purpose**: Make the API production-ready to support the wtd-app frontend  
**Date**: December 3, 2025  
**Status**: Planning Phase

---

## 📋 Executive Summary

The wtd-app frontend is a fully functional React SPA currently using localStorage and mock data. To support the frontend, this API needs to implement:

1. **Core missing functionality** - Bet model, DbContext, authentication endpoints
2. **RESTful API endpoints** matching frontend expectations
3. **JWT authentication** for secure user sessions
4. **Database persistence** replacing frontend's localStorage
5. **CORS configuration** for frontend communication

### Business Impact
- ✅ **Immediate**: Replace localStorage with persistent database storage
- ✅ **Security**: Implement proper authentication and authorization
- ✅ **Multi-device**: Enable users to access data from any device
- ✅ **Scale**: Support multiple users with isolated show balances
- ✅ **Foundation**: Enable future real-time features (WebSocket, notifications)

---

## 🎯 Frontend Requirements Analysis

### Frontend Architecture (from wtd-app)
```
React 19 SPA
├── AuthStore (Zustand)
│   ├── user: { id, username, email }
│   ├── isAuthenticated: boolean
│   ├── showBalances: { [showId]: balance }
│   ├── login(username, password)
│   ├── logout()
│   └── updateCurrency(showId, amount)
│
└── BetStore (Zustand)
    ├── bets: Bet[]
    ├── placeBet(betData)
    ├── resolveBet(betId, won)
    └── getUserBets(userId, episodeId?)
```

### Expected API Endpoints (Derived from Frontend)

#### Authentication
- `POST /api/auth/register` - Create new user account
- `POST /api/auth/login` - Authenticate user, return JWT
- `GET /api/auth/me` - Get current user profile
- `POST /api/auth/logout` - Invalidate session

#### Shows
- `GET /api/shows` - List all active shows
- `GET /api/shows/{id}` - Get show details with seasons and characters
- `POST /api/shows/{id}/join` - User joins a show (creates UserShow with initial balance)

#### Characters
- `GET /api/shows/{showId}/characters` - List characters for a show
- `GET /api/characters/{id}` - Get character details
- `GET /api/shows/{showId}/characters/alive` - List alive characters only

#### Episodes
- `GET /api/seasons/{seasonId}/episodes` - List episodes for a season
- `GET /api/episodes/{id}` - Get episode details
- `PATCH /api/episodes/{id}/betting` - Toggle betting open/closed (admin)

#### Bets
- `GET /api/bets/me` - Get current user's bets
- `GET /api/bets/me/{episodeId}` - Get user's bets for specific episode
- `POST /api/bets` - Place a new bet
- `GET /api/shows/{showId}/leaderboard` - Get user rankings by balance

#### User Balance
- `GET /api/users/me/shows/{showId}/balance` - Get balance for a show
- `PATCH /api/users/me/shows/{showId}/balance` - Update balance (system use)

---

## 🚧 Missing Components (Critical Path)

### 1. **Bet Model** ❌ MISSING
**Status**: Referenced in other models but not implemented  
**Priority**: CRITICAL  
**Blocking**: All betting functionality

**Required fields** (from architecture docs):
```csharp
public class Bet
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CharacterId { get; set; }
    public int EpisodeId { get; set; }
    public decimal Amount { get; set; }
    public string Prediction { get; set; } // "dies" or "survives"
    public string Status { get; set; } // "pending", "won", "lost", "refunded"
    public DateTime PlacedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    
    // Navigation
    public User User { get; set; }
    public Character Character { get; set; }
    public Episode Episode { get; set; }
}
```

### 2. **DbContext** ❌ MISSING
**Status**: Not created  
**Priority**: CRITICAL  
**Blocking**: All database operations

### 3. **Authentication Service** ❌ MISSING
**Status**: Not implemented  
**Priority**: CRITICAL  
**Blocking**: User login, secure endpoints

### 4. **DTOs (Data Transfer Objects)** ❌ EMPTY
**Status**: Folder exists but empty  
**Priority**: HIGH  
**Blocking**: Proper API responses, security (password hiding)

### 5. **Endpoints** ❌ EMPTY
**Status**: Folder exists but empty  
**Priority**: HIGH  
**Blocking**: All API functionality

### 6. **Services** ❌ EMPTY
**Status**: Folder exists but empty  
**Priority**: HIGH  
**Blocking**: Business logic

### 7. **Database Connection String** ❌ NOT CONFIGURED
**Status**: appsettings.json needs PostgreSQL connection  
**Priority**: CRITICAL  
**Blocking**: Application startup

---

## 📦 Implementation Phases

### **Phase 1: Foundation (Week 1)** 🏗️

#### 1.1 Database Setup
- [x] Create `ApplicationDbContext` with all DbSets - DONE
- [x] Configure entity relationships and indexes - DONE
- [x] Add connection string to appsettings.json - DONE
- [ ] Create initial migration
- [ ] Seed database with sample shows/characters

**Files to create:**
- `Data/ApplicationDbContext.cs`
- `Data/DbSeeder.cs`

**Files to modify:**
- `appsettings.json` (add ConnectionStrings section)
- `appsettings.Development.json` (add dev database connection)

#### 1.2 Bet Model
- [x] Create `Models/Bet.cs` - DONE
- [x] Add validation attributes - DONE
- [ ] Update migration

#### 1.3 DTOs
- [x] `DTOs/Auth/RegisterRequest.cs` - DONE
- [x] `DTOs/Auth/LoginRequest.cs` - DONE
- [x] `DTOs/Auth/LoginResponse.cs` - DONE
- [x] `DTOs/Auth/UserProfileDto.cs` - DONE
- [x] `DTOs/Shows/ShowDto.cs` - DONE
- [x] `DTOs/Shows/ShowDetailDto.cs` - DONE
- [x] `DTOs/Characters/CharacterDto.cs` - DONE
- [x] `DTOs/Episodes/EpisodeDto.cs` - DONE
- [x] `DTOs/Bets/BetDto.cs` - DONE
- [x] `DTOs/Bets/PlaceBetRequest.cs` - DONE
- [x] `DTOs/Bets/BetResultDto.cs` - DONE
- [x] `DTOs/Leaderboard/LeaderboardEntryDto.cs` - DONE

---

### **Phase 2: Authentication & Security (Week 1-2)** 🔐

#### 2.1 JWT Service
- [x] Create `Services/IJwtService.cs` - DONE
- [x] Create `Services/JwtService.cs` - DONE
- [x] Add JWT configuration to appsettings.json - DONE
- [x] Register service in Program.cs - DONE

**JWT Settings:**
```json
"Jwt": {
  "SecretKey": "your-256-bit-secret-key-here-min-32-chars",
  "Issuer": "WillTheyDieApi",
  "Audience": "WillTheyDieApp",
  "ExpirationMinutes": 1440
}
```

#### 2.2 Authentication Service
- [x] Create `Services/IAuthService.cs` - DONE
- [x] Create `Services/AuthService.cs` - DONE
- [x] Implement user registration with BCrypt password hashing - DONE
- [x] Implement login with token generation - DONE
- [x] Implement password validation - DONE

#### 2.3 Authentication Middleware
- [x] Configure JWT bearer authentication in Program.cs - DONE
- [ ] Add CORS policy for frontend
- [ ] Create authorization policies

**CORS Settings:**
```json
"AllowedOrigins": [
  "http://localhost:5173",
  "http://localhost:3000",
  "https://your-frontend-domain.com"
]
```

---

### **Phase 3: Core Business Services (Week 2)** 🏢

#### 3.1 Show Service
- [x] Create `Services/IShowService.cs` - DONE
- [x] Create `Services/ShowService.cs` - DONE
- [x] Implement GetActiveShows() - DONE
- [x] Implement GetShowById(id) - DONE
- [x] Implement JoinShow(userId, showId) - creates UserShow - DONE

#### 3.2 Character Service
- [ ] Create `Services/ICharacterService.cs`
- [ ] Create `Services/CharacterService.cs`
- [ ] Implement GetCharactersByShow(showId)
- [ ] Implement GetAliveCharacters(showId)
- [ ] Implement GetCharacterById(id)

#### 3.3 Episode Service
- [ ] Create `Services/IEpisodeService.cs`
- [ ] Create `Services/EpisodeService.cs`
- [ ] Implement GetEpisodesBySeason(seasonId)
- [ ] Implement GetEpisodeById(id)
- [ ] Implement ToggleBetting(episodeId, isOpen) - admin only

#### 3.4 Bet Service
- [x] Create `Services/IBetService.cs` - DONE
- [x] Create `Services/BetService.cs` - DONE
- [x] Implement PlaceBet(userId, betData) - DONE
  - Validate episode betting is open
  - Validate character is alive
  - Validate user has sufficient balance
  - Deduct balance from UserShow
  - Create bet record
- [x] Implement GetUserBets(userId, episodeId?) - DONE
- [ ] Implement ResolveBet(betId, won) - admin only
  - Update bet status
  - Update user balance if won
- [ ] Implement GetUserStats(userId, showId)

#### 3.5 Leaderboard Service
- [ ] Create `Services/ILeaderboardService.cs`
- [ ] Create `Services/LeaderboardService.cs`
- [ ] Implement GetShowLeaderboard(showId, limit)
- [ ] Sort by balance descending

---

### **Phase 4: API Endpoints (Week 2-3)** 🌐

#### 4.1 Authentication Endpoints
- [x] Create `Endpoints/AuthEndpoints.cs` - DONE
- [x] `POST /api/auth/register` - DONE
- [x] `POST /api/auth/login` - DONE
- [x] `GET /api/auth/me` (requires auth) - DONE
- [ ] `POST /api/auth/logout` (optional - JWT is stateless)

#### 4.2 Show Endpoints
- [x] Create `Endpoints/ShowEndpoints.cs` - DONE
- [x] `GET /api/shows` - DONE
- [x] `GET /api/shows/{id}` - DONE
- [x] `POST /api/shows/{id}/join` (requires auth) - DONE

#### 4.3 Character Endpoints
- [ ] Create `Endpoints/CharacterEndpoints.cs`
- [ ] `GET /api/shows/{showId}/characters`
- [ ] `GET /api/characters/{id}`
- [ ] `GET /api/shows/{showId}/characters/alive`

#### 4.4 Episode Endpoints
- [ ] Create `Endpoints/EpisodeEndpoints.cs`
- [ ] `GET /api/seasons/{seasonId}/episodes`
- [ ] `GET /api/episodes/{id}`
- [ ] `PATCH /api/episodes/{id}/betting` (requires admin auth)

#### 4.5 Bet Endpoints
- [x] Create `Endpoints/BetEndpoints.cs` - DONE
- [x] `GET /api/bets/me` (requires auth) - DONE
- [x] `GET /api/bets/me/{episodeId}` (requires auth) - DONE
- [x] `POST /api/bets` (requires auth) - DONE

#### 4.6 Leaderboard Endpoints
- [ ] Create `Endpoints/LeaderboardEndpoints.cs`
- [ ] `GET /api/shows/{showId}/leaderboard`

#### 4.7 User Balance Endpoints
- [x] Create `Endpoints/UserEndpoints.cs` - DONE
- [x] `GET /api/users/me/shows/{showId}/balance` (requires auth) - DONE

---

### **Phase 5: Integration & Testing (Week 3)** 🧪

#### 5.1 Database Migrations
- [ ] Run all migrations
- [ ] Seed test data
- [ ] Verify relationships

#### 5.2 API Testing
- [ ] Test authentication flow
- [ ] Test show listing and joining
- [ ] Test bet placement flow
- [ ] Test balance updates
- [ ] Test leaderboard
- [ ] Create `WillTheyDie.Api.http` test file

#### 5.3 Frontend Integration
- [ ] Update frontend API base URL
- [ ] Replace localStorage with API calls
- [ ] Test authentication flow
- [ ] Test betting flow
- [ ] Test error handling

---

### **Phase 6: Polish & Documentation (Week 3-4)** ✨

#### 6.1 Error Handling
- [ ] Create `Middleware/ExceptionHandlingMiddleware.cs`
- [ ] Implement global error handler
- [ ] Return consistent error responses

#### 6.2 Validation
- [ ] Add FluentValidation package (optional)
- [ ] Add validation to all request DTOs
- [ ] Return clear validation errors

#### 6.3 Logging
- [ ] Configure structured logging
- [ ] Log authentication attempts
- [ ] Log bet placements
- [ ] Log errors with context

#### 6.4 API Documentation
- [ ] Configure Swagger/OpenAPI
- [ ] Add XML documentation comments
- [ ] Add request/response examples
- [ ] Test all endpoints in Swagger UI

---

## 🗂️ File Structure (Target State)

```
WillTheyDie.Api/
├── Data/
│   ├── ApplicationDbContext.cs ✅
│   ├── DbSeeder.cs ✅
│   └── Migrations/ ✅
│
├── Models/
│   ├── User.cs ✅ (existing)
│   ├── Show.cs ✅ (existing)
│   ├── Season.cs ✅ (existing)
│   ├── Episode.cs ✅ (existing)
│   ├── Character.cs ✅ (existing)
│   ├── UserShow.cs ✅ (existing)
│   └── Bet.cs ❌ TO CREATE
│
├── DTOs/
│   ├── Auth/
│   │   ├── RegisterRequest.cs
│   │   ├── LoginRequest.cs
│   │   ├── LoginResponse.cs
│   │   └── UserProfileDto.cs
│   ├── Shows/
│   │   ├── ShowDto.cs
│   │   └── ShowDetailDto.cs
│   ├── Characters/
│   │   └── CharacterDto.cs
│   ├── Episodes/
│   │   └── EpisodeDto.cs
│   ├── Bets/
│   │   ├── BetDto.cs
│   │   ├── PlaceBetRequest.cs
│   │   └── BetResultDto.cs
│   └── Leaderboard/
│       └── LeaderboardEntryDto.cs
│
├── Services/
│   ├── IJwtService.cs
│   ├── JwtService.cs
│   ├── IAuthService.cs
│   ├── AuthService.cs
│   ├── IShowService.cs
│   ├── ShowService.cs
│   ├── ICharacterService.cs
│   ├── CharacterService.cs
│   ├── IEpisodeService.cs
│   ├── EpisodeService.cs
│   ├── IBetService.cs
│   ├── BetService.cs
│   ├── ILeaderboardService.cs
│   └── LeaderboardService.cs
│
├── Endpoints/
│   ├── AuthEndpoints.cs
│   ├── ShowEndpoints.cs
│   ├── CharacterEndpoints.cs
│   ├── EpisodeEndpoints.cs
│   ├── BetEndpoints.cs
│   ├── LeaderboardEndpoints.cs
│   └── UserBalanceEndpoints.cs
│
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs
│
├── Program.cs (major updates needed)
├── appsettings.json (add ConnectionStrings, Jwt, CORS)
└── appsettings.Development.json (add dev overrides)
```

---

## 🎯 Critical Path Items (Must Complete First)

### Week 1 Priority Order:
1. ✅ **Bet Model** - Everything depends on this
2. ✅ **ApplicationDbContext** - Can't run without database
3. ✅ **Connection String** - Required for EF Core
4. ✅ **Initial Migration** - Creates database schema
5. ✅ **JWT Service** - Required for authentication
6. ✅ **AuthService** - User login/register
7. ✅ **Auth Endpoints** - Frontend needs this first

### Week 2 Priority Order:
1. ✅ **Show Service + Endpoints** - Browse available shows
2. ✅ **Character Service + Endpoints** - See who to bet on
3. ✅ **Episode Service + Endpoints** - Know when to bet
4. ✅ **Bet Service + Endpoints** - Core functionality
5. ✅ **Leaderboard Service + Endpoints** - Competitive element

---

## 📊 Frontend Integration Checklist

### Replace localStorage with API calls:

#### AuthStore Changes:
```typescript
// OLD: localStorage
const user = JSON.parse(localStorage.getItem('user') || 'null');

// NEW: API call
const response = await fetch('/api/auth/me', {
  headers: { 'Authorization': `Bearer ${token}` }
});
const user = await response.json();
```

#### BetStore Changes:
```typescript
// OLD: localStorage
const bets = JSON.parse(localStorage.getItem('bets') || '[]');

// NEW: API call
const response = await fetch('/api/bets/me');
const bets = await response.json();
```

#### Show Balances:
```typescript
// OLD: localStorage per user
showBalances: { "1": 5000, "2": 3200 }

// NEW: API endpoint
GET /api/users/me/shows/1/balance
=> { "showId": 1, "balance": 5000 }
```

---

## 🔒 Security Considerations

### Password Security
- ✅ BCrypt hashing with salt (already in dependencies)
- ✅ Minimum 8 characters
- ✅ Never return password hash in DTOs

### JWT Security
- ✅ HTTPS only in production
- ✅ Secure secret key (min 256 bits)
- ✅ Short expiration (24 hours)
- ✅ Include user ID and username in claims

### Authorization
- ✅ Validate user owns the bet before allowing actions
- ✅ Validate user has joined show before placing bets
- ✅ Admin-only endpoints for episode/character management
- ✅ Rate limiting for bet placement (future)

### CORS
- ✅ Whitelist specific frontend origins
- ✅ Allow credentials for cookie-based auth (if used)
- ✅ Restrict allowed methods/headers

---

## 🎲 Betting Logic Rules

### Bet Placement Validation:
1. ✅ Episode betting must be open
2. ✅ Character must be alive (status = "alive")
3. ✅ User must have joined the show (UserShow exists)
4. ✅ User must have sufficient balance
5. ✅ Bet amount must be > 0
6. ✅ Prediction must be "dies" or "survives"
7. ✅ User can't bet same character/prediction for same episode twice

### Bet Resolution Logic:
1. ✅ Admin marks character status after episode airs
2. ✅ System resolves bets:
   - Character died + prediction "dies" = WON
   - Character died + prediction "survives" = LOST
   - Character survived + prediction "dies" = LOST
   - Character survived + prediction "survives" = WON
3. ✅ Calculate payout (simple 2x for MVP, odds-based later)
4. ✅ Update UserShow balance
5. ✅ Set bet status to "won" or "lost"
6. ✅ Record ResolvedAt timestamp

---

## 🗄️ Database Schema

### Connection String Example:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=willtheydie_dev;Username=postgres;Password=your_password"
  }
}
```

### Indexes (for performance):
```csharp
// ApplicationDbContext.OnModelCreating
modelBuilder.Entity<User>()
    .HasIndex(u => u.Username).IsUnique();
    
modelBuilder.Entity<User>()
    .HasIndex(u => u.Email).IsUnique();
    
modelBuilder.Entity<UserShow>()
    .HasIndex(us => new { us.UserId, us.ShowId }).IsUnique();
    
modelBuilder.Entity<Character>()
    .HasIndex(c => new { c.ShowId, c.Status });
    
modelBuilder.Entity<Bet>()
    .HasIndex(b => new { b.UserId, b.EpisodeId });
    
modelBuilder.Entity<Bet>()
    .HasIndex(b => new { b.EpisodeId, b.Status });
```

---

## 🧪 Sample Test Data (Seed)

### Shows:
1. Game of Thrones
2. The Walking Dead
3. Breaking Bad

### Characters (Game of Thrones):
1. Jon Snow (alive)
2. Daenerys Targaryen (alive)
3. Ned Stark (dead) - historical
4. Tyrion Lannister (alive)

### Seasons/Episodes:
- Season 1, Episodes 1-10
- Episode 1: Betting open
- Episode 2: Betting closed (already aired)

### Users:
- testuser1 / password123
- testuser2 / password123

---

## 📈 Success Metrics

### Technical:
- ✅ All endpoints respond < 200ms (without caching)
- ✅ 100% of DTOs exclude sensitive data
- ✅ Zero N+1 query issues (use .Include())
- ✅ All endpoints have Swagger documentation

### Functional:
- ✅ User can register and login
- ✅ User can browse shows and characters
- ✅ User can join a show and receive initial balance
- ✅ User can place bet and balance updates
- ✅ User can view bet history
- ✅ Leaderboard shows top 10 users per show

### Frontend Integration:
- ✅ Frontend removes all localStorage references
- ✅ Authentication persists across page reloads (token in localStorage)
- ✅ Multi-device login works (JWT stateless)
- ✅ Error messages from API display properly

---

## 🚀 Deployment Readiness

### Prerequisites:
- [ ] PostgreSQL database (Azure PostgreSQL / AWS RDS / local)
- [ ] Environment variables configured
- [ ] CORS origins whitelisted
- [ ] SSL/TLS certificate (production)
- [ ] Database migrations applied

### Environment Variables (Production):
```bash
ConnectionStrings__DefaultConnection="Host=prod-db;Database=willtheydie;..."
Jwt__SecretKey="production-secret-key-min-32-characters"
Jwt__Issuer="WillTheyDieApi"
Jwt__Audience="WillTheyDieApp"
AllowedOrigins__0="https://willtheydie.com"
```

---

## 🔄 Next Steps After MVP

### Phase 7: Caching (use existing feature1.plan.md)
- Redis distributed cache
- Cache shows, characters, leaderboards
- 50% reduction in database queries

### Phase 8: Real-time Features
- SignalR for live bet updates
- WebSocket for leaderboard changes
- Push notifications for episode air dates

### Phase 9: Advanced Features
- Odds calculation based on betting patterns
- Bet recommendations (ML)
- Social features (friend leagues)
- Achievement system

---

## 📚 References

### Frontend Architecture:
- `C:\Users\rmathis\source\wtd-app\architecture.md`

### API Architecture:
- `C:\Users\rmathis\source\wtd-api\architecture.md`

### Existing Plan:
- `C:\Users\rmathis\source\wtd-api\feature1.plan.md` (Redis/Azure App Config)

### ASP.NET Core Docs:
- [Minimal APIs](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis)
- [JWT Authentication](https://learn.microsoft.com/aspnet/core/security/authentication/jwt-authn)
- [EF Core](https://learn.microsoft.com/ef/core/)

---

## ✅ Definition of Done

### Phase 1-4 Complete When:
- [ ] All models created (including Bet)
- [ ] DbContext configured and migration applied
- [ ] All 6 service interfaces/implementations created
- [ ] All 7 endpoint files created with routes
- [ ] All DTOs created (15+ files)
- [ ] JWT authentication working
- [ ] CORS configured for frontend
- [ ] Swagger UI accessible and documented

### Frontend Integration Complete When:
- [ ] Frontend successfully authenticates
- [ ] Frontend can place bets via API
- [ ] Frontend displays API-sourced data
- [ ] localStorage removed except for JWT token
- [ ] Error handling works end-to-end

### Production Ready When:
- [ ] PostgreSQL database deployed
- [ ] Connection strings in environment variables
- [ ] HTTPS enforced
- [ ] Rate limiting implemented
- [ ] Monitoring/logging configured
- [ ] Backup strategy in place

---

**Plan Author**: AI Assistant (Claude)  
**Estimated Effort**: 3-4 weeks for Phases 1-6  
**Complexity**: Medium-High  
**Risk Level**: Medium (database schema changes, frontend integration)  
**Recommended Start**: Phase 1 - Foundation (immediately)
