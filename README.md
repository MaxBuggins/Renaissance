# Renaissance
Multiplayer First Person Shooter Project, first attempt at implimenting Networking Code.

Features/Pros
+ Math oriented movement and projectile system 
+ Server sided scoreing
+ UDP and TCP
+ Funky graphics (Subjective)
+ Client sided sounds and Visual Effects (Clients decide depending on players movements weather to play a jump/walk sound instead the server manually telling them)


Known Problems/Cons 
- Known spawning exploits (players will only be able to spawn hit boxs and projectiles when alive and relative to where they standing according to the server, however when coniditons met they can request an infinite number of spawns (They can spawn a million bullets)).
- Clients can only directly connect to servers via an IP address (this wont be fixed as I dont have a dedicated server to mess with)
- Large maps size (map sizes have been fixed however it requires them being made from scratch hence there is still some maps that are too big (snow castle map is a whole Gigabite)
- Some shaders overa particals in front of them (Water Bowl is exspecially prown to the water shader overalaping smoke particals (Fixed from moveing partical rendering to GPU based visual shader instead of the CPU partical system however this change requires particals to be remade from scratch)).
- Hit Boxs may not register if moveing faster or deleted before than the server can keep up (projectiles dont have this problem because of math approach).
- Limited menu options (Press a map and host a game)
