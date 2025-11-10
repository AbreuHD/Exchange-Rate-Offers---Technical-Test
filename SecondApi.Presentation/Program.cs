using SecondApi.Presentation.Services;
using SoapCore;
using SoapCore.Extensibility;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IExchangeService, ExchangeService>();
builder.Services.AddSingleton<IFaultExceptionTransformer, DefaultFaultExceptionTransformer<CustomMessage>>();

var app = builder.Build();

app.UseRouting();
((IApplicationBuilder)app).UseSoapEndpoint<IExchangeService>(
    "/Service.asmx",
    new SoapEncoderOptions(),
    SoapSerializer.DataContractSerializer
);

app.MapGet("/", () => "SOAP Service running!");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
