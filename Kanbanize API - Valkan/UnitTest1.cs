using RestSharp;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using RestSharp.Authenticators;



namespace Kanbanize_API___Valkan
{
    public class API_CardTests
    {

        private RestClient client;
        private const string username = "apikey";
        private const string password = "L5fUBq4gH5F5PiDLZ0UfGcjnJIVf4SoAY0S2htGW";
        private const string baseUrl = "https://valkaneod.kanbanize.com";
        private const string partialUrl = "api/v2/cards";
        public const int cardIdUrl = 184 + 1;
       // public CardResponse card;

        //Важна информация преди пускането на тестовете!!!
        // 1-во: пускаме само Test 1 "Create Card"
        // 2-ро: гледаме:"Standart Output", от където разбираме номера на създадената карта.
        // 3-то: въвеждаме номера на създадената карта в полето над информацията: 
        // "public const int cardIdUrl и заменямае първото число с номера на картата"
        // по този начин при стартиране на всички тестове, новата създадена карта ще е с ID:
        // ID-то на първоначални създадената карта + 1, тъй като ще се създаде нова карта,
        // върху която се правят всички останали тестове.
        // 4-то: стартираме всички тестове.




        [SetUp]
        public void Setup()
        {
            this.client = new RestClient(baseUrl);

        }
       

        [Test,Order (1)]
        public void Crate_Card()
        {
       
       
            Body body = CreateNewBody(1, 1, "Test title Valkan", 0, "#256D7B", 250);
       
            var request = CreateRequest(partialUrl, Method.Post, body, 2000);
       
            var response = client.Execute(request);
       
       
       
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);
       
            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
           

            Console.WriteLine($"Card_Id = {responseBody.data[0].card_id}");     

        }

        [Test, Order(2)]
        public void CreateCardCheck()
        {

            
            var request = new RestRequest(partialUrl + $"/{cardIdUrl}", Method.Get);

            request.AddHeader(username, password);

            var response = client.Execute(request);

            var responseBody = JsonSerializer.Deserialize<CardResponseObject>(response.Content);


            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(responseBody.data.title, Is.EqualTo("Test title Valkan"));
            Assert.That(responseBody.data.position, Is.EqualTo(0));
            Assert.That(responseBody.data.color.ToLower(), Is.EqualTo("256D7B".ToLower()));
            Assert.That(responseBody.data.priority, Is.EqualTo(250));


        }


        [Test, Order(3)]
        public void Test_ТryToCrateCardWithDifferentWorkflowOfLaneIdAndColumnId()
        {
            Body body = CreateNewBody(2, 1, "Test title Valkan", 0, "#256D7B", 250);
            var request = CreateRequest(partialUrl, Method.Post, body, 2000);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorBox<int>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The lane and column of " +
                "a card must be in the same workflow. The lane with id 2 and the column " +
                "with id 1 are not."));
        }

        [Test, Order(4)]
        public void Test_ТryToCrateCardWithLaneIdEqualToZero()
        {
            Body body = CreateNewBody(0, 1, "Test title Valkan", 0, "#256D7B", 250);
            var request = CreateRequest(partialUrl, Method.Post, body, 2000);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorBox<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request" +
                " body did not pass validation."));
            Assert.That(responseError.error.details.lane_id[0], Is.EqualTo("The value must be a positive number."));
        }



        [Test, Order(5)]
        public void Test_ТryToCrateCardWithMissingLaneId()
        {
            Body body = CreateNewBody(null, 1, "Test title Valkan", 0, "#256D7B", 250);
            var request = CreateRequest(partialUrl, Method.Post, body, 2000);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorBox<int>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            Assert.That(responseError.error.message, Is.EqualTo("Please provide a lane_id for the new " +
                "card with reference CCR or have it copied from an " +
                "existing card by using card_properties_to_copy."));
        }

        [Test, Order(6)]
        public void Test_ТryToCrateCardWithLaneIdEqualsThree()
        {
            Body body = CreateNewBody(3, 1, "Test title Valkan", 0, "#256D7B", 250);
            var request = CreateRequest(partialUrl, Method.Post, body, 2000);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorBox<int>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            Assert.That(responseError.error.message, Is.EqualTo("A lane with id 3 does not exist."));
        }




        [Test, Order(7)]
        public void Test_ТryToCrateCardWithMissingColumnId()
        {
            Body body = CreateNewBody(1, null, "Test title Valkan", 0, "#256D7B", 250);
            var request = CreateRequest(partialUrl, Method.Post, body, 2000);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorBox<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            Assert.That(responseError.error.message, Is.EqualTo("Please provide a column_id" +
                " for the new card with reference CCR or have it copied" +
                " from an existing card by using card_properties_to_copy."));

        }
        [Test, Order(8)]
        public void Test_ТryToCrateCardWithColumnIdEqualsZeo()
        {
            Body body = CreateNewBody(1, 0, "Test title Valkan", 0, "#256D7B", 250);
            var request = CreateRequest(partialUrl, Method.Post, body, 2000);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorBox<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request" +
                " body did not pass validation."));

            Assert.That(responseError.error.details.column_id[0], Is.EqualTo("The value must be a positive number."));

        }

        [Test, Order(9)]
        public void Test_ТryToCrateCardWithColumnIdEqualsFive()
        {
            Body body = CreateNewBody(1, 5, "New Test Title", 0, "#256D7B", 250);
            var request = CreateRequest(partialUrl, Method.Post, body, 2000);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseBody.data[0].lane_id, Is.EqualTo(1));
            Assert.That(responseBody.data[0].column_id, Is.EqualTo(5));
            Assert.That(responseBody.data[0].title, Is.EqualTo("New Test Title"));
            Assert.That(responseBody.data[0].position, Is.EqualTo(0));
            Assert.That(responseBody.data[0].color.ToLower(), Is.EqualTo("256D7B".ToLower()));
            Assert.That(responseBody.data[0].priority, Is.EqualTo(250));

        }

        [Test, Order(10)]
        public void Test_ТryToCrateCardWithColumnIdEqualsSix()
        {
            Body body = CreateNewBody(1, 6, "New Test Title", 0, "#256D7B", 250);
            var request = CreateRequest(partialUrl, Method.Post, body, 2000);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorBox<int>>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            Assert.That(responseError.error.message, Is.EqualTo("The lane and column of a card" +
                " must be in the same workflow. The lane with id 1 and the column with id 6 are not."));

        }

        [Test, Order(11)]
        public void Test_ТryToCrateCardWithoutTitle()
        {
            Body body = CreateNewBody(1, 1, "", 0, "#256D7B", 250);
            var request = CreateRequest(partialUrl, Method.Post, body, 2000);
            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseBody.data[0].lane_id, Is.EqualTo(1));
            Assert.That(responseBody.data[0].column_id, Is.EqualTo(1));
            Assert.That(responseBody.data[0].title, Is.EqualTo(""));
            Assert.That(responseBody.data[0].position, Is.EqualTo(0));
            Assert.That(responseBody.data[0].color.ToLower(), Is.EqualTo("256D7B".ToLower()));
            Assert.That(responseBody.data[0].priority, Is.EqualTo(250));

        }
        [Test, Order(12)]
        public void Test_ТryToCrateCardWithIntegerTitle()
        {

            var request = new RestRequest(partialUrl, Method.Post);

            var reqBody = new
            {
                lane_id = 1,
                column_id = 1,
                title = 1234,
                position = 0,
                color = "#256D7B",
                priority = 250
            };

            request.AddBody(reqBody);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);


            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseBody.data[0].lane_id, Is.EqualTo(1));
            Assert.That(responseBody.data[0].column_id, Is.EqualTo(1));
            Assert.That(responseBody.data[0].title, Is.EqualTo("1234"));
            Assert.That(responseBody.data[0].position, Is.EqualTo(0));
            Assert.That(responseBody.data[0].color.ToLower(), Is.EqualTo("256D7B".ToLower()));
            Assert.That(responseBody.data[0].priority, Is.EqualTo(250));

        }
        [Test, Order(13)]
        public void Test_ТryToCrateCardWitInvalidPosition()
        {
            Body body = CreateNewBody(1, 1, "New Test Title", -1, "#256D7B", 250);
            var request = CreateRequest(partialUrl, Method.Post, body, 2000);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorBox<List<string>>>(response.Content);
            
            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request " +
                "body did not pass validation."));
            Assert.That(responseError.error.details.position[0], Is.EqualTo("The value must be " +
                "greater than or equal to zero."));

        }
        [Test, Order(14)]
        public void Test_ТryToCrateCardWitInvalidPositionString()
        {
            var request = new RestRequest(partialUrl, Method.Post);

            var reqBody = new
            {
                lane_id = 1,
                column_id = 1,
                title = "New Test Title",
                position = "position",
                color = "#256D7B",
                priority = 250
            };

            request.AddBody(reqBody);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);


            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorBox<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request " +
                "body did not pass validation."));
            Assert.That(responseError.error.details.position[0], Is.EqualTo("The value must be an integer."));

        }
        [Test, Order(15)]
        public void Test_ТryToCrateCardWithIntegerColor()
        {

            var request = new RestRequest(partialUrl, Method.Post);

            var reqBody = new
            {
                lane_id = 1,
                column_id = 1,
                title = "New Title",
                position = 0,
                color = 256890,
                priority = 250
            };

            request.AddBody(reqBody);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);


            var response = client.Execute(request);
            var responseBody = JsonSerializer.Deserialize<CardResponse>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseBody.data[0].lane_id, Is.EqualTo(1));
            Assert.That(responseBody.data[0].column_id, Is.EqualTo(1));
            Assert.That(responseBody.data[0].title, Is.EqualTo("New Title"));
            Assert.That(responseBody.data[0].position, Is.EqualTo(0));
            Assert.That(responseBody.data[0].color.ToLower(), Is.EqualTo("256890"));
            Assert.That(responseBody.data[0].priority, Is.EqualTo(250));

        }
        [Test, Order(16)]
        public void Test_ТryToCrateCardWithInvalidColor()
        {

            var request = new RestRequest(partialUrl, Method.Post);

            var reqBody = new
            {
                lane_id = 1,
                column_id = 1,
                title = "New Title",
                position = 0,
                color = -1,
                priority = 250
            };

            request.AddBody(reqBody);
            request.AddParameter("application/json", reqBody, ParameterType.RequestBody);
            request.AddHeader(username, password);


            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorBox<List<string>>>(response.Content);
           
            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.details.color[0], Is.EqualTo("The value must be a valid " +
                "6-character or 3-character color string."));

        }
        [Test, Order(17)]
        public void Test_ТryToCrateCardWitInvalidPriority()
        {
            Body body = CreateNewBody(1, 1, "New Test Title", 0, "#256D7B", 300);
            var request = CreateRequest(partialUrl, Method.Post, body, 2000);
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorBox<List<string>>>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request " +
                "body did not pass validation."));
            Assert.That(responseError.error.details.priority[0], Is.EqualTo("The value must be between 1 and 250."));

        }



        [Test, Order(18)]
        public void Test_ТryToCrateCardWithOppositeVariables()
        {
            BodyWithOppositeVariables body = CreateNewBodyWithOppositeVariables("lane_id", "column_id", 123, "position", 12345, "priority");

            var request = new RestRequest(partialUrl, Method.Post);

            request.AddParameter("application/json", body, ParameterType.RequestBody);
            request.AddHeader(username, password);
            request.Timeout = 2000;
            var response = client.Execute(request);
            var responseError = JsonSerializer.Deserialize<ErrorBox<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request" +
                " body did not pass validation."));

            Assert.That(responseError.error.details.column_id[0], Is.EqualTo("The value must be an integer."));
            Assert.That(responseError.error.details.lane_id[0], Is.EqualTo("The value must be an integer."));
            Assert.That(responseError.error.details.position[0], Is.EqualTo("The value must be an integer."));
            Assert.That(responseError.error.details.priority[0], Is.EqualTo("The value must be an integer."));
        }




        //Преместване на картата на нова позиция


        [Test, Order(19)]
        public void Update_PrimaryCardWithoutAddingOtherCards()
        {
            
            var body = CreateNewBody(1, 1, "Test title Valkan", 1, "#256D7B", 250);
            var request = CreateRequest(partialUrl + $"/{cardIdUrl}", Method.Patch, body, 2000);
            var response = client.Execute(request);

            var responseEditedBody = JsonSerializer.Deserialize<CardResponse>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
          
        }

        // Проверяваме в нова заявка дали позицията е преместена

        [Test, Order(20)]
        public void Update_PrimaryCardWithoutAddingOtherCardsCheck()
        {
            

            var request = new RestRequest(partialUrl + $"/{cardIdUrl}", Method.Get);
            request.AddHeader(username, password);

            var response = client.Execute(request);

            var responseBody = JsonSerializer.Deserialize<Body>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseBody.position, Is.EqualTo(null));              //<---- Получаваме, че не можем да преместим картата от нулева поцизия,
                                                                                 //ако преди това не сме създали нова карта.
        }

        [Test, Order(21)]
        public void Update_PrimaryCardWitAddingNewCard()
        {
            // Създаваме нова карта на позиция 1, тъй като основната ни карта е на позиция 0.

            Body bodyNew = CreateNewBody(1, 1, "New Card Test", 1, "#256D7B", 250);

            var newRequest = CreateRequest(partialUrl, Method.Post, bodyNew, 2000);
            var newResponse = client.Execute(newRequest);

            //Променяме основната си карта на позиция 1 и така новата създадена карта, отива на поцияия 0.

            var body = CreateNewBody(1, 1, "Test title Valkan", 1, "#256D7B", 250);
            var request = CreateRequest(partialUrl + $"/{cardIdUrl}", Method.Patch, body, 2000);
            var response = client.Execute(request);
            
            var responseEditedBody = JsonSerializer.Deserialize<CardResponse>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));



        }
        //Проверяваме дали позицията на основната ни карта е променена

        [Test, Order(22)]
        public void Update_PrimaryCardWitAddingNewCardCheck()
        {


            var request = new RestRequest(partialUrl + $"/{cardIdUrl}", Method.Get);
            request.AddHeader(username, password);

            var response = client.Execute(request);

            var responseBody = JsonSerializer.Deserialize<CardResponseObject>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseBody.data.position, Is.EqualTo(1));              // След като вече има друга създадена карта
        }                                                              // успяхме да променим позицията на основната карта.

        [Test, Order(23)]
        public void TryToUpdate_PrimaryCardWithIncorectPosition()
        {

            var body = CreateNewBody(1, 1, "Test title Valkan", -1, "#256D7B", 250);
            var request = CreateRequest(partialUrl + $"/{cardIdUrl}", Method.Patch, body, 2000);
            var response = client.Execute(request);

            var responseError = JsonSerializer.Deserialize<ErrorBox<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseError.error.message, Is.EqualTo("The parameters in the request " +
                "body did not pass validation."));
            Assert.That(responseError.error.details.position[0], Is.EqualTo("The value must be greater than or equal to zero."));


        }
        [Test, Order(24)]
        public void TryToUpdate_PrimaryCardWithIncocectCardId()
        {

            var body = CreateNewBody(1, 1, "Test title Valkan", 0, "#256D7B", 250);
            var request = CreateRequest(partialUrl + $"/600", Method.Patch, body, 2000);
            var response = client.Execute(request);

            var responseError = JsonSerializer.Deserialize<ErrorBox<List<string>>>(response.Content);

            Assert.That(response.ContentType.StartsWith("application/json"), Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(responseError.error.message, Is.EqualTo("A card with id 600 does not exist."));

        }

        [Test, Order(25)]
        public void Delete_PrimaryCard()
     {
    
         var request = new RestRequest(partialUrl + $"/{cardIdUrl}", Method.Delete);
         request.AddHeader(username, password);
         request.Timeout = 2000;
         
         var response = client.Execute(request);
        
         Assert.That(response.StatusCode,Is.EqualTo(HttpStatusCode.NoContent));
  

     }
        [Test, Order(26)]
        public void Delet_PrimaryCardWitAddingNewCardCheck()
     {
    
    
         var request = new RestRequest(partialUrl + $"/{cardIdUrl}", Method.Get);
         request.AddHeader(username, password);
         request.Timeout = 2000;
    
         var response = client.Execute(request);
    
         Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    
    
     }
    
    

        private Body CreateNewBody(int? _lane_id, int? _column_id, string _title, int? _position, string _color, int? _priority)
        {
            var body = new Body
            {
                lane_id = _lane_id,
                column_id = _column_id,
                title = _title,
                position = _position,
                color = _color,
                priority = _priority
            };

            return body;
        }
        private BodyWithOppositeVariables CreateNewBodyWithOppositeVariables(string _lane_id, string _column_id, int? _title, string _position, int? _color, string _priority)
        {
            var body = new BodyWithOppositeVariables
            {
                lane_id = _lane_id,
                column_id = _column_id,
                title = _title,
                position = _position,
                color = _color,
                priority = _priority
            };

            return body;
        }


        private RestRequest CreateRequest(string url, Method method, Body body, int timeout)
        {
            var request = new RestRequest(url, method);

            request.AddParameter("application/json", body, ParameterType.RequestBody);
            request.AddHeader(username, password);
            request.Timeout = timeout;

            return request;

            
        }

        public void CardCreation()
        {

            Body body = CreateNewBody(1, 1, "Card Valkan", 0, "#256D7B", 250);

            var request = CreateRequest(partialUrl, Method.Post, body, 2000);

            var response = client.Execute(request);



        }


        
    }
}
   










