using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using APL_Technical_Test.helper;
using Microsoft.AspNetCore.Hosting;
using APL_Technical_Test.repository;
using APL_Technical_Test.data.entities;
using APL_Technical_Test.Azure;

namespace APL_Technical_Test.Pages
{
    public class IndexModel : PageModel
    {
        [ViewData]
        public bool isData { get; set; }
        [ViewData]
        public string imagePath { get; set; }
        [ViewData]
        public string imageName { get; set; }
        public string wrongImageTypeErrorMessage { get; private set; }
        [ViewData]
        public string imageOverSizeMessage { get; set; }
        [BindProperty]
        public IFormFile UploadImage { get; set; }
        [ViewData]
        public ImageInformation imageInfor { get; set; }
        private DateTime imageCreatedDate { get; set; }
        [ViewData]
        public string ImageSaveFailedMessage { get; private set; }
        [ViewData]
        public string noImageFoundError { get; private set; }
        [ViewData]
        public string serverExceptionErrorMessage { get; private set; }
        public bool errorMessage { get; private set; }

        private readonly ILogger<IndexModel> _logger;
       private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IImageRepository repo;

        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment environment, IImageRepository repository)
        {
            this.isData = false;
            _logger = logger;
            _hostEnvironment = environment;
            repo = repository;
           
        }
        //public IActionResult OnGet(ImageInformation data)
        //{



        //}

        public void OnGet()
        {
            
        
        }
           
           
        //}
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (this.UploadImage != null)
            {
                try
                {
                    if (ImageUploadHelper.CheckIfJpgOrPngFile(this.UploadImage))
                    {
                        if (await ImageUploadHelper.WriteFile(this.UploadImage, _hostEnvironment))
                        {
                            this.imagePath = ImageUploadHelper.filePath;
                            //=================================get image details====================================================
                            System.Drawing.Image img = System.Drawing.Image.FromFile(this.imagePath);
                            int width = img.Width;
                            int height = img.Height;
                            if(width == 1024 && height == 1024)
                            {
                                // get images infor
                                this.imageName = this.UploadImage.FileName;
                               

                                this.imageInfor = new ImageInformation();
                                this.imageInfor.Dimensions = width.ToString() + "x" + height.ToString();
                                this.imageInfor.ImageName = this.imageName;
                                this.imageInfor.UploadDate = DateTime.Now;
                                var findObjectById = this.imageInfor.UploadDate;
                                string mimeType = UploadImage.ContentType;
                                byte[] fileData = new byte[UploadImage.Length];
                                ViewData["mimetype"] = mimeType;
                                ViewData["fileData"] = fileData;
                                Azure.Service.AzureStorageService objBlobService = new Azure.Service.AzureStorageService();
                                var imagePathOnAzure = objBlobService.UploadFileToBlob(this.imageName, fileData, mimeType);
                                if (imagePathOnAzure != "")
                                {
                                    this.errorMessage = false;
                                    this.imageInfor.AzureUrl = imagePathOnAzure;
                                    this.repo.Add(this.imageInfor);
                                    if (this.repo.save() > 0)
                                    {
                                       // this.imageInfor = repo.GetById(findObjectById);
                                        return Page();
                                    }
                                    else
                                    {
                                        this.errorMessage = true;
                                        this.imageName = null;
                                        this.ImageSaveFailedMessage = "Your Upload Failed";
                                        return Page();
                                    }

                                   
                                }
                                else
                                {
                                    this.imageName = null;
                                    this.ImageSaveFailedMessage = "You Upload failed";
                                    return Page();
                                }
                       
                            }
                            else
                            {
                                this.imageName = null;
                                this.imageOverSizeMessage = "Your Image size must be 1024X1024 only";

                                return Page();
                            }
                            
                               
                        }
                        else
                        {
                            this.imageName = null;
                            this.ImageSaveFailedMessage = "You Upload failed";
                            return Page();
                        }
                        
                    }
                    else
                    {
                        this.imageName = null;
                        this.wrongImageTypeErrorMessage = "You can only upload jpg and png files";
                        return Page();
                    }
                    
                }
                catch(Exception e)
                {
                    this.imageName = null;
                    this.serverExceptionErrorMessage = "Server error try latter";
                    return Page();
                }

               

            }
            else
            {
                this.noImageFoundError = "You have no image";
                return Page();

            }


            {



            }
        }
    } 
}
