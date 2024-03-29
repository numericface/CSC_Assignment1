﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;

namespace s3ImageUpload
{
    
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            // Our Code Will Go Here
            // Display the Image
            {
                System.IO.Stream stream = FileUpload1.PostedFile.InputStream;
                System.IO.BinaryReader br = new System.IO.BinaryReader(stream);
                Byte[] bytes = br.ReadBytes((Int32)stream.Length);
                string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                Image1.ImageUrl = "data:image/jpeg;base64," + base64String;
                Image1.Visible = true;
                GetObjectResponse getResponse;
                var bucketName = "p1828737csc2";

                // Store to S3
                {
                    // Do not actually store your IAM credentials in code. EC2 Role is best
                    var awsKey = "";
                    var awsSecretKey = "";
                    var bucketRegion = Amazon.RegionEndpoint.APSoutheast1;   // Your bucket region
                    var s3 = new AmazonS3Client(awsKey, awsSecretKey, bucketRegion);
                    var putRequest = new PutObjectRequest();
                    putRequest.BucketName = bucketName;        // Your bucket name
                    putRequest.ContentType = "image/jpeg";
                    putRequest.InputStream = FileUpload1.PostedFile.InputStream;// key will be the name of the image in your bucket
                    putRequest.Key = FileUpload1.FileName;
                    putRequest.CannedACL = S3CannedACL.PublicRead;
                    PutObjectResponse putResponse = s3.PutObject(putRequest);


                }

                var imageUrl = "https://" + bucketName + ".s3-" + Amazon.RegionEndpoint.APSoutheast1.SystemName + ".amazonaws.com/" + FileUpload1.FileName;
              //  var imageUrl = "https://p1828737csc2.s3.amazonaws.com/WIN_20191106_15_20_07_Pro.jpg";
                ImageURL.Text = imageUrl;
                ImageURL.Visible = true;

                String bitely = ShortenUrl(imageUrl).Result;
                //String bitely = "bit.ly/2Bdommq";
                String custom = CustomBackHalf(bitely, FileUpload1.FileName).Result;
                Debug.WriteLine(custom);
                //method to make custom back half
                //change below
                bitlyURL.Text = bitely;
                bitlyURL.Visible = true;
            }
        }

        public static async Task<string> ShortenUrl(string url)
        {
            string _bitlyToken = "560c63bf3029b375974070eea7ebaf4042c8782a";
            HttpClient client = new HttpClient();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post,
                "https://api-ssl.bitly.com/v4/shorten")
            {
                Content = new StringContent($"{{\"long_url\":\"{url}\"}}",
                                                Encoding.UTF8,
                                                "application/json")
            };

            try
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _bitlyToken);

                var response = await client.SendAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                    return string.Empty;

                var responsestr = await response.Content.ReadAsStringAsync();

                dynamic jsonResponse = JsonConvert.DeserializeObject<dynamic>(responsestr);
                return jsonResponse["link"];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return string.Empty;
            }
        }

        public static async Task<string> CustomBackHalf(string bitly, string filename)
        {
            string _bitlyToken = "BITLY_TOKEN";
            HttpClient client = new HttpClient();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post,
                "https://api-ssl.bitly.com/v4/custom_bitlinks")
            {
                Content = new StringContent($"{{\"bitlink_id\":\"{bitly}\",\"custom_bitlink\":\"bit.ly/huehuehue\"}}",
                                                Encoding.UTF8,
                                                "application/json")
            };

            try
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _bitlyToken);
                Debug.WriteLine(request.Content.ReadAsStringAsync().Result);
                var response = await client.SendAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                    return string.Empty;

                var responsestr = await response.Content.ReadAsStringAsync();

                dynamic jsonResponse = JsonConvert.DeserializeObject<dynamic>(responsestr);
                return jsonResponse["link"];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return string.Empty;
            }
        }
    }


}