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















