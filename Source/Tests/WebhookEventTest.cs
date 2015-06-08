﻿using System;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PayPal.Api;

namespace PayPal.Testing
{
    [TestClass]
    public class WebhookEventTest
    {
        public static readonly string WebhookEventJson = 
            "{\"id\":\"8PT597110X687430LKGECATA\"," +
	        "\"create_time\":\"2013-06-25T21:41:28Z\"," +
	        "\"resource_type\":\"authorization\"," +
            "\"event_type\":\"PAYMENT.AUTHORIZATION.CREATED\"," +
	        "\"summary\":\"A payment authorization was created\"," +
	        "\"resource\":" + AuthorizationTest.AuthorizationJson + "," +
	        "\"links\":[{" +
		        "\"href\":\"https://api.sandbox.paypal.com/v1/notfications/webhooks-events/8PT597110X687430LKGECATA\"," +
		        "\"rel\":\"self\"," +
		        "\"method\":\"GET\"" +
	        "},{" +
		        "\"href\":\"https://api.sandbox.paypal.com/v1/notfications/webhooks-events/8PT597110X687430LKGECATA/resend\"," +
		        "\"rel\":\"resend\"," +
		        "\"method\":\"POST\"}]}";

        public static WebhookEvent GetWebhookEvent()
        {
            return JsonFormatter.ConvertFromJson<WebhookEvent>(WebhookEventJson);
        }

        [TestMethod, TestCategory("Unit")]
        public void WebhookEventObjectTest()
        {
            var testObject = GetWebhookEvent();
            Assert.AreEqual("8PT597110X687430LKGECATA", testObject.id);
            Assert.AreEqual("2013-06-25T21:41:28Z", testObject.create_time);
            Assert.AreEqual("authorization", testObject.resource_type);
            Assert.AreEqual("PAYMENT.AUTHORIZATION.CREATED", testObject.event_type);
            Assert.AreEqual("A payment authorization was created", testObject.summary);
            Assert.IsNotNull(testObject.resource);
            Assert.IsNotNull(testObject.links);
            Assert.IsTrue(testObject.links.Count == 2);
        }

        [TestMethod, TestCategory("Unit")]
        public void WebhookEventConvertToJsonTest()
        {
            Assert.IsFalse(GetWebhookEvent().ConvertToJson().Length == 0);
        }

        [TestMethod, TestCategory("Unit")]
        public void WebhookEventToStringTest()
        {
            Assert.IsFalse(GetWebhookEvent().ToString().Length == 0);
        }

        [Ignore]
        public void WebhookEventGetTest()
        {
            var webhookEventId = "8PT597110X687430LKGECATA";
            var webhookEvent = WebhookEvent.Get(TestingUtil.GetApiContext(), webhookEventId);
            Assert.IsNotNull(webhookEvent);
            Assert.AreEqual(webhookEventId, webhookEvent.id);
        }

        [Ignore]
        public void WebhookEventGetAllTest()
        {
            var webhookEventList = WebhookEvent.List(TestingUtil.GetApiContext());
            Assert.IsNotNull(webhookEventList);
        }

        [TestMethod, TestCategory("Unit")]
        public void WebhookEventValidateSupportedAuthAlgorithm()
        {
            Assert.AreEqual("SHA1", WebhookEvent.ConvertAuthAlgorithmHeaderToHashAlgorithmName("SHA1withRSA"));
            Assert.AreEqual("SHA256", WebhookEvent.ConvertAuthAlgorithmHeaderToHashAlgorithmName("SHA256withRSA"));
            Assert.AreEqual("SHA512", WebhookEvent.ConvertAuthAlgorithmHeaderToHashAlgorithmName("SHA512withRSA"));
            Assert.AreEqual("MD5", WebhookEvent.ConvertAuthAlgorithmHeaderToHashAlgorithmName("MD5withRSA"));
        }

        [TestMethod, TestCategory("Unit")]
        public void WebhookEventValidateNotSupportedAuthAlgorithm()
        {
            TestingUtil.AssertThrownException<AlgorithmNotSupportedException>(() => WebhookEvent.ConvertAuthAlgorithmHeaderToHashAlgorithmName("SHA1withDSA"));
            TestingUtil.AssertThrownException<AlgorithmNotSupportedException>(() => WebhookEvent.ConvertAuthAlgorithmHeaderToHashAlgorithmName("SHA256withDSA"));
            TestingUtil.AssertThrownException<AlgorithmNotSupportedException>(() => WebhookEvent.ConvertAuthAlgorithmHeaderToHashAlgorithmName("SHA512withDSA"));
            TestingUtil.AssertThrownException<AlgorithmNotSupportedException>(() => WebhookEvent.ConvertAuthAlgorithmHeaderToHashAlgorithmName("MD5withDSA"));
        }

        [TestMethod, TestCategory("Functional")]
        public void WebhookEventValidateReceivedEventInValidTest()
        {
            var requestBody = "{\"id\":\"WH-83M739899B401212M-7DU699748W928720N\",\"create_time\":\"2015-01-20T21:36:29Z\",\"resource_type\":\"sale\",\"event_type\":\"PAYMENT.SALE.COMPLETED\",\"summary\":\"Payment completed for $ 100.0 USD\",\"resource\":{\"id\":\"2BK99536JB384163F\",\"create_time\":\"2015-01-20T21:35:18Z\",\"update_time\":\"2015-01-20T21:35:37Z\",\"amount\":{\"total\":\"100.00\",\"currency\":\"USD\"},\"payment_mode\":\"INSTANT_TRANSFER\",\"state\":\"completed\",\"protection_eligibility\":\"ELIGIBLE\",\"protection_eligibility_type\":\"ITEM_NOT_RECEIVED_ELIGIBLE,UNAUTHORIZED_PAYMENT_ELIGIBLE\",\"parent_payment\":\"PAY-29E10708MU8063339KS7MUFQ\",\"links\":[{\"href\":\"https://10.72.108.213:11881/v1/payments/sale/2BK99536JB384163F\",\"rel\":\"self\",\"method\":\"GET\"},{\"href\":\"https://10.72.108.213:11881/v1/payments/sale/2BK99536JB384163F/refund\",\"rel\":\"refund\",\"method\":\"POST\"},{\"href\":\"https://10.72.108.213:11881/v1/payments/payment/PAY-29E10708MU8063339KS7MUFQ\",\"rel\":\"parent_payment\",\"method\":\"GET\"}]},\"links\":[{\"href\":\"https://api.sandbox.paypal.com/v1/notifications/webhooks-events/WH-83M739899B401212M-7DU699748W928720N\",\"rel\":\"self\",\"method\":\"GET\"},{\"href\":\"https://api.sandbox.paypal.com/v1/notifications/webhooks-events/WH-83M739899B401212M-7DU699748W928720N/resend\",\"rel\":\"resend\",\"method\":\"POST\"}]}";
            var requestHeaders = new NameValueCollection
            {
                {"Paypal-Cert-Url", "https://api.sandbox.paypal.com/v1/notifications/certs/CERT-360caa42-35c2ed1e-21e9a5d6"},
                {"Paypal-Auth-Version", "v2"},
                {"Paypal-Transmission-Sig", "UsYAipVbZNtuSGq59AqYhxA/k5esWWE8RGsaYHUQpURsrnyfpPou7AaozlMRXbP2Ry+REChStJLdjCeYHQa2PZAAr3ikzPFCT5kSNd1hL52fIXW60l3k5fJIMo4qSpvXmraEhn3zXAPIaw11RzzUxrDe4wDql4yhot109H+ZtFNUSjOt/KzzZugpAwwIfwOgtnbjpLhfRJaMykozeMVelBPQ8GaYUpK37QI3BFYGs0joEEXeiRWZLAbIbyxBe1xYF8oVCYmhP47fkwhkHjy1J0hK8mhFPxOe1/6WKbljiJ9jbHzkdOWeInBhbq8LbaCX1Q+fHPkKOdo/bimMM5Pw4Q=="},
                {"Paypal-Transmission-Id", "657044e0-a0ec-11e4-a003-6b62a8a99ac4"},
                {"Paypal-Auth-Algo", "SHA1withRSA"},
                {"Correlation-Id", "a31e7d8667bd5"},
                {"Paypal-Transmission-Time", "2015-01-20T21:36:30Z"}
            };
            var webhookId = "6XE614444P001923J";
            var apiContext = TestingUtil.GetApiContext();
            Assert.IsFalse(WebhookEvent.ValidateReceivedEvent(apiContext, requestHeaders, requestBody, webhookId));
        }

        [TestMethod, TestCategory("Functional")]
        public void WebhookEventValidateReceivedEventValidTest()
        {
            var requestBody = "{\"id\":\"WH-2W7266712B616591M-36507203HX6402335\",\"create_time\":\"2015-05-12T18:14:14Z\",\"resource_type\":\"sale\",\"event_type\":\"PAYMENT.SALE.COMPLETED\",\"summary\":\"Payment completed for $ 20.0 USD\",\"resource\":{\"id\":\"7DW85331GX749735N\",\"create_time\":\"2015-05-12T18:13:18Z\",\"update_time\":\"2015-05-12T18:13:36Z\",\"amount\":{\"total\":\"20.00\",\"currency\":\"USD\"},\"payment_mode\":\"INSTANT_TRANSFER\",\"state\":\"completed\",\"protection_eligibility\":\"ELIGIBLE\",\"protection_eligibility_type\":\"ITEM_NOT_RECEIVED_ELIGIBLE,UNAUTHORIZED_PAYMENT_ELIGIBLE\",\"parent_payment\":\"PAY-1A142943SV880364LKVJEFPQ\",\"transaction_fee\":{\"value\":\"0.88\",\"currency\":\"USD\"},\"links\":[{\"href\":\"https://api.sandbox.paypal.com/v1/payments/sale/7DW85331GX749735N\",\"rel\":\"self\",\"method\":\"GET\"},{\"href\":\"https://api.sandbox.paypal.com/v1/payments/sale/7DW85331GX749735N/refund\",\"rel\":\"refund\",\"method\":\"POST\"},{\"href\":\"https://api.sandbox.paypal.com/v1/payments/payment/PAY-1A142943SV880364LKVJEFPQ\",\"rel\":\"parent_payment\",\"method\":\"GET\"}]},\"links\":[{\"href\":\"https://api.sandbox.paypal.com/v1/notifications/webhooks-events/WH-2W7266712B616591M-36507203HX6402335\",\"rel\":\"self\",\"method\":\"GET\"},{\"href\":\"https://api.sandbox.paypal.com/v1/notifications/webhooks-events/WH-2W7266712B616591M-36507203HX6402335/resend\",\"rel\":\"resend\",\"method\":\"POST\"}]}";
            var requestHeaders = new NameValueCollection
            {
                {"Paypal-Cert-Url", "https://api.sandbox.paypal.com/v1/notifications/certs/CERT-360caa42-fca2a594-a5cafa77"},
                {"Paypal-Auth-Version", "v2"},
                {"Paypal-Transmission-Sig", "vSOIQFIZQHv8G2vpbOpD/4fSC4/MYhdHyv+AmgJyeJQq6q5avWyHIe/zL6qO5hle192HSqKbYveLoFXGJun2od2zXN3Q45VBXwdX3woXYGaNq532flAtiYin+tQ/0pNwRDsVIufCxa3a8HskaXy+YEfXNnwCSL287esD3HgOHmuAs0mYKQdbR4e8Evk8XOOQaZzGeV7GNXXz19gzzvyHbsbHmDz5VoRl9so5OoHqvnc5RtgjZfG8KA9lXh2MTPSbtdTLQb9ikKYnOGM+FasFMxk5stJisgmxaefpO9Q1qm3rCjaJ29aAOyDNr3Q7WkeN3w4bSXtFMwyRBOF28pJg9g=="},
                {"Paypal-Transmission-Id", "b2384410-f8d2-11e4-8bf3-77339302725b"},
                {"Paypal-Auth-Algo", "SHA256withRSA"},
                {"Correlation-Id", "a31e7d8667bd5"},
                {"Paypal-Transmission-Time", "2015-05-12T18:14:14Z"}
            };
            var webhookId = "3RN13029J36659323";
            var apiContext = TestingUtil.GetApiContext();
            Assert.IsTrue(WebhookEvent.ValidateReceivedEvent(apiContext, requestHeaders, requestBody, webhookId));
        }

        [TestMethod, TestCategory("Functional")]
        public void WebhookEventValidateReceivedEventInvalidTrustedCertificatePathTest()
        {
            var requestBody = "{\"id\":\"WH-2W7266712B616591M-36507203HX6402335\",\"create_time\":\"2015-05-12T18:14:14Z\",\"resource_type\":\"sale\",\"event_type\":\"PAYMENT.SALE.COMPLETED\",\"summary\":\"Payment completed for $ 20.0 USD\",\"resource\":{\"id\":\"7DW85331GX749735N\",\"create_time\":\"2015-05-12T18:13:18Z\",\"update_time\":\"2015-05-12T18:13:36Z\",\"amount\":{\"total\":\"20.00\",\"currency\":\"USD\"},\"payment_mode\":\"INSTANT_TRANSFER\",\"state\":\"completed\",\"protection_eligibility\":\"ELIGIBLE\",\"protection_eligibility_type\":\"ITEM_NOT_RECEIVED_ELIGIBLE,UNAUTHORIZED_PAYMENT_ELIGIBLE\",\"parent_payment\":\"PAY-1A142943SV880364LKVJEFPQ\",\"transaction_fee\":{\"value\":\"0.88\",\"currency\":\"USD\"},\"links\":[{\"href\":\"https://api.sandbox.paypal.com/v1/payments/sale/7DW85331GX749735N\",\"rel\":\"self\",\"method\":\"GET\"},{\"href\":\"https://api.sandbox.paypal.com/v1/payments/sale/7DW85331GX749735N/refund\",\"rel\":\"refund\",\"method\":\"POST\"},{\"href\":\"https://api.sandbox.paypal.com/v1/payments/payment/PAY-1A142943SV880364LKVJEFPQ\",\"rel\":\"parent_payment\",\"method\":\"GET\"}]},\"links\":[{\"href\":\"https://api.sandbox.paypal.com/v1/notifications/webhooks-events/WH-2W7266712B616591M-36507203HX6402335\",\"rel\":\"self\",\"method\":\"GET\"},{\"href\":\"https://api.sandbox.paypal.com/v1/notifications/webhooks-events/WH-2W7266712B616591M-36507203HX6402335/resend\",\"rel\":\"resend\",\"method\":\"POST\"}]}";
            var requestHeaders = new NameValueCollection
            {
                {"Paypal-Cert-Url", "https://api.sandbox.paypal.com/v1/notifications/certs/CERT-360caa42-fca2a594-a5cafa77"},
                {"Paypal-Auth-Version", "v2"},
                {"Paypal-Transmission-Sig", "vSOIQFIZQHv8G2vpbOpD/4fSC4/MYhdHyv+AmgJyeJQq6q5avWyHIe/zL6qO5hle192HSqKbYveLoFXGJun2od2zXN3Q45VBXwdX3woXYGaNq532flAtiYin+tQ/0pNwRDsVIufCxa3a8HskaXy+YEfXNnwCSL287esD3HgOHmuAs0mYKQdbR4e8Evk8XOOQaZzGeV7GNXXz19gzzvyHbsbHmDz5VoRl9so5OoHqvnc5RtgjZfG8KA9lXh2MTPSbtdTLQb9ikKYnOGM+FasFMxk5stJisgmxaefpO9Q1qm3rCjaJ29aAOyDNr3Q7WkeN3w4bSXtFMwyRBOF28pJg9g=="},
                {"Paypal-Transmission-Id", "b2384410-f8d2-11e4-8bf3-77339302725b"},
                {"Paypal-Auth-Algo", "SHA256withRSA"},
                {"Correlation-Id", "a31e7d8667bd5"},
                {"Paypal-Transmission-Time", "2015-05-12T18:14:14Z"}
            };
            var webhookId = "3RN13029J36659323";
            var apiContext = TestingUtil.GetApiContext();
            apiContext.Config[BaseConstants.TrustedCertificateLocation] = @"C:\invalid\path\to\trusted\certificate.cer";
            TestingUtil.AssertThrownException<PayPalException>(() => WebhookEvent.ValidateReceivedEvent(apiContext, requestHeaders, requestBody, webhookId));
        }
    }
}
