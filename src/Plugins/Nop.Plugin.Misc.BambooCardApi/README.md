## BambooCardApi plugin - Nop.Plugin.Misc.BambooCardApi:

This plugin provides two API endpoints:

• Authentication using JWT

• Retrieve customer orders by email

### Authentication

To generate a JWT token, send a request to:

http://localhost:8080/api/authenticate?email=your@email.com

![image](https://github.com/user-attachments/assets/567cb9a7-fe36-40b1-a340-a3d8663bf151)


### Retrieve Customer Orders
To fetch a customer's orders, use the following endpoint:

http://localhost:8080/api/BambooOrders/customerOrders?email=admin@geleg.com&pageIndex=1&pageSize=5

Make sure to:

• Set query parameters (email, pageIndex, pageSize)

• Add the Bearer Token from the authentication step in the Authorization tab in
Postman

![image](https://github.com/user-attachments/assets/db4a49dc-c841-4d7c-b89a-3ed32503dc3e)

Then in the authorization tab select Bearer token and paste the token generated from the
authentication step

![image](https://github.com/user-attachments/assets/cbfb4cb7-3358-4951-97f7-5bc4592d974d)

### Postman Collection
A Postman collection file is included for testing:

BambooCard Api.postman_collection.json
