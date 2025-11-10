# Exchange Rate Offers

Sistema de APIs distribuido en C# .NET para consultar y comparar tasas de cambio entre diferentes proveedores de divisas.

## 📖 ¿Cómo Funciona?

El sistema utiliza una **arquitectura de microservicios** donde la **CentralAPI** actúa como orquestador principal:

1. **Cliente** realiza una solicitud de conversión de divisas a la **CentralAPI**
2. **CentralAPI** consulta simultáneamente todas las fuentes disponibles:
   - FirstAPI (proveedor interno 1)
   - SecondAPI (proveedor interno 2)
   - ThirdAPI (proveedor interno 3)
   - Frankfurter API (servicio externo)
   - Floatrates API (servicio externo)
3. **CentralAPI** recibe y compara las tasas de cambio (rates) de cada proveedor
4. **CentralAPI** devuelve:
   - **Best Rate**: La mejor tasa de cambio encontrada
   - **All Rates**: Todas las tasas para comparación manual

Este enfoque garantiza que siempre obtengas la mejor oferta disponible del mercado en tiempo real.

## 🚀 Características

* **Arquitectura de Microservicios**: 4 APIs independientes (CentralAPI + 3 proveedores)
* **APIs Externas**: Integración con Frankfurter y Floatrates para tasas de cambio en tiempo real
* **Mejor Oferta**: Consulta automática de la mejor tasa entre todos los proveedores
* **Comparación de Tasas**: Obtén todas las tasas disponibles para comparar
* **Consultas Múltiples**: Procesa varias conversiones simultáneamente
* **Contenerización**: Todas las APIs ejecutándose en Docker

## 📋 Requisitos

* Docker Desktop instalado
* Docker Compose v2.0 o superior
* .NET 9.0 SDK (solo si deseas desarrollar/compilar fuera de Docker)

## 🔧 Instalación y Ejecución

### Opción 1: Construcción y Ejecución

Construye las imágenes y ejecuta todos los contenedores:

```bash
docker-compose up --build
```

### Opción 2: Ejecución en Segundo Plano (Modo Detached)

Para ejecutar los servicios en segundo plano:

```bash
docker-compose up -d
```

Para detener los servicios:

```bash
docker-compose down
```

## 📦 Exportar Imágenes Docker

Si necesitas exportar todas las imágenes para transferirlas a otro servidor:

```bash
docker save -o exchange-rate-offers-all.tar exchange-rate-offers-centralapi:latest exchange-rate-offers-firstapi:latest exchange-rate-offers-secondapi:latest exchange-rate-offers-thirdapi:latest
```

## 🌐 APIs y Puertos

Una vez iniciados los contenedores, las APIs estarán disponibles en:

| Servicio | Puerto | URL |
|----------|--------|-----|
| **CentralAPI** | 5260 | http://localhost:5260 |
| FirstAPI | 5150 | http://localhost:5150 |
| SecondAPI | 5042 | http://localhost:5042 |
| ThirdAPI | 5107 | http://localhost:5107 |

### Documentación Swagger

Accede a la documentación interactiva de cada API:

* CentralAPI: http://localhost:5260/swagger
* FirstAPI: http://localhost:5150/swagger
* SecondAPI: http://localhost:5042/swagger
* ThirdAPI: http://localhost:5107/swagger

## 🚀 Demo en Vivo

La **CentralAPI** está hosteada y disponible para pruebas en línea:

**URL de la API hosteada**: https://techtest-banreservas.abreuhd.com/swagger/index.html

Puedes probar todos los endpoints directamente desde el navegador sin necesidad de instalar Docker localmente.

## 🧪 Códigos de Moneda Soportados

El sistema soporta códigos ISO 4217 estándar:

* **USD** - Dólar estadounidense
* **EUR** - Euro
* **GBP** - Libra esterlina
* **JPY** - Yen japonés
* **CAD** - Dólar canadiense
* **AUD** - Dólar australiano
* **CHF** - Franco suizo
* **MXN** - Peso mexicano
* Y muchos más...

## 🌍 APIs Externas Integradas

El sistema consulta las siguientes APIs externas de terceros para obtener tasas de cambio en tiempo real:

### 1. Frankfurter API
* **URL**: https://frankfurter.app
* **Descripción**: API gratuita de tasas de cambio publicada por el Banco Central Europeo
* **Características**: 
  - Datos actualizados diariamente
  - Más de 30 monedas soportadas
  - Sin necesidad de autenticación

### 2. Floatrates API
* **URL**: https://www.floatrates.com
* **Descripción**: API pública que proporciona tasas de cambio de múltiples divisas
* **Características**:
  - Datos en formato JSON
  - Actualización frecuente
  - Amplia cobertura de monedas internacionales

Estas APIs externas se combinan con las 3 APIs internas (FirstAPI, SecondAPI, ThirdAPI) para proporcionar **5 fuentes diferentes** de tasas de cambio, garantizando siempre la mejor oferta disponible.

## 📝 Ejemplos de Uso

### Obtener la Mejor Oferta

```bash
curl -X GET "http://localhost:5260/api/exchange/best-rate?from=USD&to=EUR&amount=100"
```

### Obtener Todas las Tasas

```bash
curl -X GET "http://localhost:5260/api/exchange/all-rates?from=USD&to=EUR&amount=100"
```
---

## 💻 Aplicación de Consola

¿Prefieres una interfaz de línea de comandos? Prueba la **aplicación de consola** que consume la Api con un menú interactivo:

🔗 **[Exchange Rate Offers - Console App](https://github.com/AbreuHD/Exchange-Rate-Offers---Console)**

Características de la app de consola:
- Menú interactivo fácil de usar
- Consulta de mejor oferta
- Comparación de todas las tasas
- Procesamiento de múltiples conversiones

---
**Desarrollado para la prueba técnica - Exchange Rate Offers**
