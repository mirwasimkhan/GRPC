# GRPC
An RPC server that checks if a number is a prime.

The solution comprises two distinct projects:

1. **Server-side Project:**
   
   This component acts as an RPC server, facilitating communication between clients and the server. It enables clients to establish a stream of requests of the `PrimeRequest` type. The server processes these requests and responds when the client signals the completion of data transmission.

2. **Client-side Project:**

   This project serves as the client application. It initiates the exchange by dispatching 10,000 requests per tick. In case the response received does not encompass all the transmitted data, the client retries the communication.

By segregating these projects, the solution achieves effective communication between clients and the server through stream-based requests, while the client component ensures robustness by reattempting data transfer if necessary.

By default the server listens on port: 7046.
