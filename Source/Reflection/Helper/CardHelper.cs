// -----------------------------------------------------------------------
// <copyright file="CardHelper.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Reflection.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Threading.Tasks;
    using AdaptiveCards;
    using Microsoft.ApplicationInsights;
    using Microsoft.Extensions.Configuration;
    using Reflection.Interfaces;
    using Reflection.Model;
    using Reflection.Repositories.FeedbackData;

    /// <summary>
    /// Card Helper.
    /// </summary>
    public class CardHelper : ICard
    {
        private readonly IConfiguration _configuration;
        private readonly TelemetryClient _telemetry;

        /// <summary>
        /// Initializes a new instance of the <see cref="CardHelper"/> class.
        /// Card Helper.
        /// </summary>
        public CardHelper(IConfiguration configuration, TelemetryClient telemetry)
        {
            _configuration = configuration;
            _telemetry = telemetry;
        }

        /// <summary>
        /// Feedback adaptive card.
        /// </summary>
        /// <param name="keyValues">Dictionary of int and FeedbackDataEntity holds the feedbacks received till now</param>
        /// <param name="reflectionId">Current reflection id</param>
        /// <returns>AdaptiveCard.</returns>
        public AdaptiveCard FeedBackCard(Dictionary<int, List<FeedbackDataEntity>> keyValues, Guid? reflectionId, string questionName)
        {
            _telemetry.TrackEvent("FeedBackCard");
            try
            {
                DirectoryInfo folderinfo = new DirectoryInfo(@"wwwroot/images/reflectimages");

                foreach (FileInfo file in folderinfo.GetFiles())
                {
                    if (Guid.Parse(file.Name.Split("@")[0]) == reflectionId)
                    {
                        file.Delete();
                    }
                }

                for (int i = 1; i <= 5; i++)
                {
                    if (!keyValues.ContainsKey(i))
                    {
                        keyValues.Add(i, new List<FeedbackDataEntity>());
                    }
                }

                var totalcount = 0;
                for (int i = 1; i <= 5; i++)
                {
                    if (keyValues.ContainsKey(i))
                    {
                        totalcount = totalcount + keyValues[i].Count;
                    }
                }

                using Bitmap thumbBMP = new Bitmap(1000, 40);
                Graphics flagGraphics = Graphics.FromImage(thumbBMP);
                var color = Brushes.White;
                var width = 0;
                var previouswidth = 0;
                if (totalcount > 0)
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        if (keyValues.ContainsKey(i))
                        {
                            if (i == 1)
                            {
                                color = Brushes.MediumSeaGreen;
                            }

                            if (i == 2)
                            {
                                color = Brushes.LightGreen;
                            }

                            if (i == 3)
                            {
                                color = Brushes.Gold;
                            }

                            if (i == 4)
                            {
                                color = Brushes.LightSalmon;
                            }

                            if (i == 5)
                            {
                                color = Brushes.Salmon;
                            }

                            width = (keyValues[i].Count * 1000) / totalcount;
                            flagGraphics.FillRectangle(color, previouswidth, 0, width, 40);
                            previouswidth = previouswidth + width + 1;
                        }
                    }
                }
                else
                {
                    color = Brushes.LightGray;
                    flagGraphics.FillRectangle(color, 0, 0, 1000, 40);
                }

                var datastring = "/Images/reflectimages/" + reflectionId + "@" + Path.GetRandomFileName().Replace(".", "") + ".png";
                string outputFileName = @"wwwroot" + datastring;

                // Use RoundedImage...
                Image roundedImage = this.RoundCorners(thumbBMP, 10, Color.Transparent);
                saveImage(roundedImage, outputFileName);

                return new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
                {
                    Body = new List<AdaptiveElement>
                {

                    new AdaptiveColumnSet
                    {
                        Columns = new List<AdaptiveColumn>()
                        {
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                VerticalContentAlignment=AdaptiveVerticalContentAlignment.Center,
                                Spacing=AdaptiveSpacing.Medium,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveImage() { Url = new Uri(_configuration["BaseUri"] + "/Images/person.png"), PixelWidth=12, PixelHeight=12, HorizontalAlignment=AdaptiveHorizontalAlignment.Center }
                                },

                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                VerticalContentAlignment=AdaptiveVerticalContentAlignment.Center,
                                Spacing=AdaptiveSpacing.Small,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock("Reflections for \""+ $"{questionName}\"") { Color = AdaptiveTextColor.Default, Size=AdaptiveTextSize.Medium, Wrap=true },
                                }
                            }
                        }
                    },
                    new AdaptiveImage() { Url = new Uri(_configuration["BaseUri"] + datastring) },
                    new AdaptiveColumnSet
                    {
                        Columns = new List<AdaptiveColumn>()
                        {
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                VerticalContentAlignment=AdaptiveVerticalContentAlignment.Center,
                                Spacing=AdaptiveSpacing.Medium,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveImage() { PixelWidth=12,PixelHeight=12, Url = new Uri(_configuration["BaseUri"] + "/images/ref1.png"),Id = "1", HorizontalAlignment = AdaptiveHorizontalAlignment.Center}
                                },
                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                VerticalContentAlignment=AdaptiveVerticalContentAlignment.Center,
                                Spacing=AdaptiveSpacing.Small,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock(keyValues[1].Count.ToString())
                                }
                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                VerticalContentAlignment=AdaptiveVerticalContentAlignment.Center,
                                Spacing=AdaptiveSpacing.Medium,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveImage() { PixelWidth=12,PixelHeight=12, Url = new Uri(_configuration["BaseUri"] + "/images/ref2.png"),Id = "2", HorizontalAlignment = AdaptiveHorizontalAlignment.Center}
                                }
                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                VerticalContentAlignment=AdaptiveVerticalContentAlignment.Center,
                                Spacing=AdaptiveSpacing.Small,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock(keyValues[2].Count.ToString())
                                }
                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                VerticalContentAlignment=AdaptiveVerticalContentAlignment.Center,
                                Spacing=AdaptiveSpacing.Medium,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveImage() { PixelWidth=12,PixelHeight=12, Url = new Uri(_configuration["BaseUri"] + "/images/ref3.png"),
                                        Style = AdaptiveImageStyle.Default, Id = "3", HorizontalAlignment=AdaptiveHorizontalAlignment.Center}
                                }
                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                VerticalContentAlignment=AdaptiveVerticalContentAlignment.Center,
                                Spacing=AdaptiveSpacing.Small,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock(keyValues[3].Count.ToString())
                                }
                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                VerticalContentAlignment=AdaptiveVerticalContentAlignment.Center,
                                Spacing=AdaptiveSpacing.Medium,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveImage() { PixelWidth=12,PixelHeight=12, Url = new Uri(_configuration["BaseUri"] + "/images/ref4.png"),
                                        Style = AdaptiveImageStyle.Default, Id = "4", HorizontalAlignment = AdaptiveHorizontalAlignment.Center }
                                }
                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                VerticalContentAlignment=AdaptiveVerticalContentAlignment.Center,
                                Spacing=AdaptiveSpacing.Small,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock(keyValues[4].Count.ToString())
                                }
                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                VerticalContentAlignment=AdaptiveVerticalContentAlignment.Center,
                                Spacing=AdaptiveSpacing.Medium,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveImage() { PixelWidth=12,PixelHeight=12, Url = new Uri(_configuration["BaseUri"] + "/images/ref5.png"),
                                        Style = AdaptiveImageStyle.Default, Id = "5", HorizontalAlignment = AdaptiveHorizontalAlignment.Center }
                                }
                            },
                            new AdaptiveColumn()
                            {
                                Width = AdaptiveColumnWidth.Auto,
                                VerticalContentAlignment=AdaptiveVerticalContentAlignment.Center,
                                Spacing=AdaptiveSpacing.Small,
                                Items = new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock(keyValues[5].Count.ToString())
                                }
                            },
                        }
                    }
                },
                    Actions = new List<AdaptiveAction>
                {
                    new AdaptiveSubmitAction()
                    {
                        Type = "Action.Submit",
                        Title = "View reflections",
                        Data =
                        new TaskModuleActionHelper.AdaptiveCardValue<TaskModuleActionDetails>()
                        {
                           Data=new TaskModuleActionDetails()
                            {
                                type ="task/fetch",
                                URL =_configuration["BaseUri"] + "/openReflections/",
                            },
                            DataJson=new DataJson
                            {
                                ReflectionId=reflectionId,
                                FeedbackId=0
                            }
                        }
                    },
                },
                };
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// write image using filestream.
        /// </summary>
        /// <param name="data">data.</param>
        /// <param name="filepath">filepath.</param>
        /// <returns></returns>
        public Task<string> saveImage(Image data, string filepath)
        {
            _telemetry.TrackEvent("saveImage");

            try
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        data.Save(memory, ImageFormat.Png);
                        byte[] bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// Feedback image modification.
        /// </summary>
        /// <param name="startImage">startImage.</param>
        /// <param name="cornerRadius">cornerRadius.</param>
        /// <param name="backgroundColor">backgroundColor.</param>
        /// <returns>Image.</returns>
        public Image RoundCorners(Image startImage, int cornerRadius, Color backgroundColor)
        {
            cornerRadius *= 2;
            Bitmap roundedImage = new Bitmap(startImage.Width, startImage.Height);
            using (Graphics g = Graphics.FromImage(roundedImage))
            {
                g.Clear(backgroundColor);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Brush brush = new TextureBrush(startImage);
                GraphicsPath gp = new GraphicsPath();
                gp.AddArc(0, 0, cornerRadius, cornerRadius, 180, 90);
                gp.AddArc(0 + roundedImage.Width - cornerRadius, 0, cornerRadius, cornerRadius, 270, 90);
                gp.AddArc(0 + roundedImage.Width - cornerRadius, 0 + roundedImage.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
                gp.AddArc(0, 0 + roundedImage.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
                g.FillPath(brush, gp);
                return roundedImage;
            }
        }

        /// <summary>
        /// New post adaptive card to create new reflection.
        /// </summary>
        /// <param name="data">This is viewModel holds the forntend data.</param>
        /// <returns>AdaptiveCard.</returns>
        public AdaptiveCard CreateNewReflect(TaskInfo data)
        {
            _telemetry.TrackEvent("CreateNewReflect");

            try
            {
                return new AdaptiveCard(new AdaptiveSchemaVersion(1, 2))
                {
                    Body = new List<AdaptiveElement>
                    {
                        new AdaptiveColumnSet
                        {
                            Columns = new List<AdaptiveColumn>()
                            {
                                 new AdaptiveColumn()
                                {
                                    Width=AdaptiveColumnWidth.Auto,
                                    Height=AdaptiveHeight.Auto,
                                    Items =new List<AdaptiveElement>()
                                                {
                                                  new AdaptiveImage(){Url=new Uri(_configuration["BaseUri"] + "/images/iconCreator.png"),PixelHeight=12, PixelWidth=12, AltText="Creator",HorizontalAlignment=AdaptiveHorizontalAlignment.Center }
                                               },
                                },
                                 new AdaptiveColumn()
                                {
                                    Width=AdaptiveColumnWidth.Auto,
                                    Height=AdaptiveHeight.Auto,
                                    Spacing=AdaptiveSpacing.Small,
                                    Items=new List<AdaptiveElement>()
                                    {
                                     new AdaptiveTextBlock("Created by "+ $"{data.postCreateBy} ") { Color = AdaptiveTextColor.Default, Size=AdaptiveTextSize.Small, Wrap=true }
                                    },
                                 },
                                 new AdaptiveColumn()
                                {
                                    Width=AdaptiveColumnWidth.Auto,
                                    Height=AdaptiveHeight.Auto,
                                    Spacing=AdaptiveSpacing.Medium,
                                    Items=new List<AdaptiveElement>()
                                    {
                                        new AdaptiveImage(){Url=new Uri(_configuration["BaseUri"] + "/images/iconPrivacy.png"),PixelHeight=12, PixelWidth=12,AltText="Privacy",HorizontalAlignment=AdaptiveHorizontalAlignment.Center,Spacing=AdaptiveSpacing.None }
                                    },
                                },
                                 new AdaptiveColumn()
                                {
                                    Width=AdaptiveColumnWidth.Auto,
                                    Height=AdaptiveHeight.Auto,
                                    Spacing=AdaptiveSpacing.Small,
                                    Items=new List<AdaptiveElement>()
                                    {
                                        new AdaptiveTextBlock($"{data.privacy}") { Color = AdaptiveTextColor.Default, Size=AdaptiveTextSize.Small, Wrap=true }
                                    },
                                }
                            }
                        },
                        new AdaptiveTextBlock($"{data.question}") { Id = ($"{data.question }"), Weight = AdaptiveTextWeight.Bolder, Size=AdaptiveTextSize.Large, Wrap=true, MaxWidth=100}
                    },
                    Actions = new List<AdaptiveAction>
                    {
                    new AdaptiveSubmitAction()
                                                {
                                                    Title=" ",
                                                    IconUrl=_configuration["BaseUri"] + "/images/Default_1.png",
                                                    Type="Action.Submit",
                                                    Data =
                                                    new TaskModuleActionHelper.AdaptiveCardValue<TaskModuleActionDetails>()
                                                    {
                                                        Data = new TaskModuleActionDetails()
                                                        {
                                                            type ="task/fetch",
                                                            URL =_configuration["BaseUri"] + "/openReflections/"
                                                        },
                                                        DataJson=new DataJson
                                                        {
                                                            ReflectionId=data.reflectionID,
                                                            FeedbackId=1
                                                        }
                                                    },
                                                    },
                    new AdaptiveSubmitAction()
                                                {
                                                    Title=" ",
                                                    IconUrl=_configuration["BaseUri"] + "/images/Default_2.png",
                                                    Type="Action.Submit",
                                                    Data =
                                                    new TaskModuleActionHelper.AdaptiveCardValue<TaskModuleActionDetails>()
                                                    {
                                                        Data = new TaskModuleActionDetails()
                                                        {
                                                            type ="task/fetch",
                                                            URL =_configuration["BaseUri"] + "/openReflections/",
                                                        },
                                                        DataJson=new DataJson
                                                        {
                                                            ReflectionId=data.reflectionID,
                                                            FeedbackId=2
                                                        }
                                                    }
                                                },
                    new AdaptiveSubmitAction()
                                                {
                                                    Title=" ",
                                                    IconUrl=_configuration["BaseUri"] + "/images/Default_3.png",
                                                    Type="Action.Submit",
                                                    Data =
                                                    new TaskModuleActionHelper.AdaptiveCardValue<TaskModuleActionDetails>()
                                                    {
                                                        Data = new TaskModuleActionDetails()
                                                        {
                                                            type ="task/fetch",
                                                            URL =_configuration["BaseUri"] + "/openReflections/",
                                                        },
                                                        DataJson=new DataJson
                                                        {
                                                            ReflectionId=data.reflectionID,
                                                            FeedbackId=3
                                                        }
                                                    }
                                                },
                    new AdaptiveSubmitAction()
                                                {
                                                    Title=" ",
                                                    IconUrl=_configuration["BaseUri"] + "/images/Default_4.png",
                                                    Type="Action.Submit",
                                                    Data =
                                                    new TaskModuleActionHelper.AdaptiveCardValue<TaskModuleActionDetails>()
                                                    {
                                                        Data = new TaskModuleActionDetails()
                                                        {
                                                            type ="task/fetch",
                                                            URL =_configuration["BaseUri"] + "/openReflections/",
                                                        },
                                                        DataJson=new DataJson
                                                        {
                                                            ReflectionId=data.reflectionID,
                                                            FeedbackId=4
                                                        }
                                                    }
                                                },
                    new AdaptiveSubmitAction()
                                                {
                                                    Title=" ",
                                                    IconUrl=_configuration["BaseUri"] + "/images/Default_5.png",
                                                    Type="Action.Submit",
                                                    Data =
                                                    new TaskModuleActionHelper.AdaptiveCardValue<TaskModuleActionDetails>()
                                                    {
                                                        Data = new TaskModuleActionDetails()
                                                        {
                                                            type ="task/fetch",
                                                            URL =_configuration["BaseUri"] + "/openReflections/",
                                                        },
                                                        DataJson=new DataJson
                                                        {
                                                            ReflectionId=data.reflectionID,
                                                            FeedbackId=5
                                                        }
                                                    }
                                                },
                    }
                };
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        public AdaptiveCard ConfirmationCard(string messageId)
        {
            _telemetry.TrackEvent("ConfirmationCard");

            try
            {
                return new AdaptiveCard(new AdaptiveSchemaVersion(1, 2))
                {
                    Body = new List<AdaptiveElement>
                    {
                       
                        new AdaptiveTextBlock("Are you sure to delete") { Id = ($"{messageId}"), Weight = AdaptiveTextWeight.Bolder, Size=AdaptiveTextSize.Large, Wrap=true, MaxWidth=100}
                    },
                    Actions = new List<AdaptiveAction>
                    {
                    new AdaptiveSubmitAction()
                                    {
                                    Title="Yes",
                                    Type="Action.Submit",
                                    Style="positive",
                                    Data =messageId,
                                    DataJson="{ \"MessageId\":"+ messageId+", \"IsDelete\": \"true\",\"Action\": \"removeposts\" }"
                                    },
                     new AdaptiveSubmitAction()
                                    {
                                    Title="No",
                                    Type="Action.Submit",
                                    Style="destructive",
                                    Data =messageId,
                                    DataJson="{ \"MessageId\":"+ messageId+", \"IsDelete\": \"false\",\"Action\": \"removeposts\" }"
                                    },
                    }
                };
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }


    }
}
