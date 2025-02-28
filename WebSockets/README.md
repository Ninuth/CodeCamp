WebSocket is bidirectional, a full-duplex protocol that is used in the same scenario of client-server communication, unlike HTTP which starts from ws:// or wss://. It is a stateful protocol, which means the connection between client and server will stay alive until it gets terminated by either party (client or server). After closing the connection by either of the client or server, the connection is terminated from both ends. [https://www.geeksforgeeks.org/what-is-web-socket-and-how-it-is-different-from-the-http/]

This project demonstrates how to build a WebSocket server and client using C#. 
The server listens for WebSocket connections, handles incoming messages, and sends responses. 
The client connects to the server, sends messages, and processes responses.
