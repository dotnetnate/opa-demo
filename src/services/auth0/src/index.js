const { OAuth2Server } = require('oauth2-mock-server');
const fs = require('fs');

async function startServer() {
  const server = new OAuth2Server();

  // Generate a new RSA key and add it to the keystore
  await server.issuer.keys.generate('RS256');

  // Use the 'service' property to listen for events
  server.service.once('beforeTokenSigning', (token, req) => {
    const timestamp = Math.floor(Date.now() / 1000);
    token.payload.tenant_id = process.env.TENANT_ID || '{00000000-0000-0000-0000-000000000000}';
    token.payload.exp = timestamp + 400;
  });

  // Read the certificate and private key
  const privateKey = fs.readFileSync('/app/certs/privatekey.pem', 'utf8');
  const certificate = fs.readFileSync('/app/certs/publickey.cer', 'utf8');

  // Create HTTPS server options
  const httpsOptions = {
    key: privateKey,
    cert: certificate,
  };

  // Start the server with HTTPS
  const host = process.env.HOST || '0.0.0.0';
  const port = 9000;

  await server.start(port, host, httpsOptions);
  console.log('Issuer URL:', server.issuer.url); // -> http://localhost:8081

  // Handle graceful shutdown
  process.on('SIGINT', async () => {
    console.log('Stopping server...');
    await server.stop();
    console.log('Server stopped.');
    process.exit(0);
  });
}

// Call the async function to start the server
startServer().catch((err) => {
  console.error('Failed to start server:', err);
});
