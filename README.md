# Merkz
A Sample Networked Twin Stick Shooter.

This Project Requires Networking, therefore step one will be to establish a UDP
signal for Transferring messages from Clients to the Server.


The Server will have several Functions.

[This will be placed once main code is done and accounts are needed]
Authentication Server:

Main Server:
	Responsibilities:
		-Act as a Central Hub for Players to Connect to.
		-A Chat Window should exist for All Players in the game who'm are connected

		-Keep Tabs on player's Actions
		
		-On Connect:
			-When a player is connected, send their Character information
				-Establish the players current equipment
				-Establish the players current inventory
				-any additional loot/equipment that should be saved
		-On Disconnect:
			-Stop Tracking the Player's Actions



		When a Mission is Selected Spawn a "Lobby"
			A Lobby is a server that handles a subset of clients
			Lobby:
				Functionality:
					Hosts X Players, sends UDP data between players for Game Logic Updates
					Runs the Pre-Selected mission


Needed Functions:
	Players Solo/Party Queue
	
	A Second Lobby specific for the Mission Contract




	"Drop in" players? NO! Merkz are given contracts, once a contract is accepted no other Merkz can join
	an inprogress Contract.



Game Session:


*Note Players who'm wish to Queue togethor will first be placed in a "Lobby"
This "Lobby" then gets populated with players for

Main Lobby (Server 1):




Section 1.1:

Party Lobby (This is where mission selects and eveyrthing occurs.) (Child Server 1):
	-By Default, each player is placed into their own personal Party Lobby
		-When one player Gets invited to another players Party Lobby,
			the Players IP etc gets XFerred to the new Party Lobby
		-When the Player Leaves/Exits/Kicked, the player gets Spawned a new
		Party Lobby, then Xferred to the Empty one.
			-If a Player opts to leave a party lobby, and the lobby is empty after
			the lobby gets terminated.

	-From this Lobby the players can select a Mission Type to perform.
	(This constantly Polls the Main Lobby for other Party Lobbies seeking the Same mission)

	Steps:
		1. Check if any Mission Lobbies Exist that are Waiting on Players.
			1A) Check if the Lobby has Room for the Players in this Party Lobby
				1AA) If Yes Add the Players to the Mission Lobby
				1AB) If No Continue Checking other Mission Lobbies
		2.	If no Mission Lobbies Exist, Create one for this Party Lobby


Mission Lobby (This is where Game Updates get passed to Players) ( Sub Server 2):
	-Once the Required players are added to this server, the Game will Launch.
	-This Server Becomes the Arbiter of all Clients and will handle the relay of all information
		between clients, for game updates etc.


	Clients Send "Requests" to the Server.

	Server Sends "Updates", the Server will send Position Updates, Damage Updates, etc.


Step 1: Network.
	Create a UDP Send/Recieve System. (Manages VoiceCom/Game Updates)
	Create a TCP Send/Recieve System. (Manages TextChat/Client Connections/Transactions)



Spawner:
	When a Player is Connected Spawn the Partyu "Thread" in a seperate window.

	For a Lobby Playe








Lobbies:
Main = Main Server Handles Max Users and spawns Y Threads needed for each Mission

	Main 		- Pre-Party Creation
		- Creates a Player Struct on Connect which is used to send/recv information
		- Contains Collections of Parties
		- Creates a Party per Player

	Party 		- Pre-Mission Join
		-

	Mission_Lobby 	- Mission Lobby + Mission Mechanics
		-
	Mission_Session


Party is used to Relay Information only to members in the Current Party.
Mission is used to Relay Information only to members in the Current Mission Lobby.



Parties only require:
	Chat between Partied Members
	Ready/UnReady


Since a Party is a "Collection" of Players to Callback to, the actual communication could be kept on Main,
and the Party is only a "Collection".

That way when a UDP Message comes in, using the TCP connection, the Player can be found and the Lobby is where the
message gets sent to.

For Now avoid all VoiP complexities.


When a Game Mission is Selected and everyones ready, follow Section 1.1 Above.

	When enough players are Created, the Mission Lobby Will Spawn the Needed Mission Thread,
		Pass the Players' Info to the Thread, tell the Players what Port to send UDP signals to,
		Get the "Fake" Handshake from each player, Start monitoring "Heartbeat" + calculate/display Ping
		Launch Game!


		Mission Game Logic:

		Server (Single Thread):
			Update Physics-
			Update Projectiles-
			Update AI Logic-
			Update AI Positions-
			Update Player Positions-

			"Should a Wait time be incorporated before an update occurs?"
				-UDP packs are sent out of order, but they are eithor sent or not.
				meaning if a packet order is implemented and one packet is sent before another
					-Request the missing packet(s)
					-Order of Packets (Look, Move, Action)
					-Since the info gets cleared upon a logic update, end might be sent after a whipe
					this might get interpreted as the other 4 missing when they are not.
					
					-A Potential fix is to not care, check what packets are missing on Read when it occurs.
						-If an action is missing and nothing got whiped Ping a Resend of the missing Packets
						-This means 1 of 2 things occurs, eithor all needed packets are sent (3 || 0) or Some
						packets are and some aren't.
						Resend Requests only occur when a packet is missing. 


			Add Round Trip Time to UDP data once localhost is dropped.
				Use Rampups for constantly lost udp packets (Lag Correction(Might aswell use TCP)):
					1x 1x 2x 2x 4x 4x 8x 8x 16x 16x 32x 32x


			Ping Checks - Append TimeStamps to each DataPacket and crosscheck the time difference to get ping.

			Game Use: UDP for game logic updates
			Game Use: Poll Main TCP player for Connection info (Connected/Ping).
				Every X Second(s) Poll for Ping.

				If player connection dropped, keep player info in Mission (this is checked when connection is re-established).

				Basically, since server updates player on info. If no connection no ping check. The game
				continues as normal.

		Client:
			(Clients send events to the Server which tell the server
			what to do on its next cycle).

			Request "Fire"
			Request "Move (Direction Vector/ Force w.e)"
			Request "Orientation"





Server Security:
	Block all ports not in use.
	Step 1: Block All Ports
	Step 2: UnBlock Main Server TCP Port
	Step 3: For each Party Created Unblock it's UDP Port
	Step 4: For Each Game Session Created Unblock its UDP/TCP Port

	Whenever a Game Session Ends, Reblock its UDP/TCP ports.
	Whenever a Lobby Suicides, Reblock its UDP/TCP ports.




