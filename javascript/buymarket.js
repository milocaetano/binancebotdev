const axios = require('axios');
const crypto = require('crypto');
const NTPClient = require("ntp-client");
const apiKey = 'GRJe42ALP0pQhtdkH968cTrBXWTwGP0Qzh7ux8AdQkpNQGwsFZMAXMMj0tWIjr9Z';
const secretKey = 'gzyCFzNDOgTyO0gk3xZYjxHtcROuCc6nFxULLdbt7qKiwJzqgqgaR1WOvhvz4WgX';

//const endpoint = 'https://api.binance.com/api/v3/order';
const endpoint = `https://testnet.binance.vision/api/v3/order`;
const symbol = 'BTCUSDT';
const side = 'BUY';
const type = 'MARKET';
const quantity = '0.001';


async function getBinanceServerTime() {
    try {
      const response = await fetch('https://testnet.binance.vision/api/v1/time');
      const data = await response.json();
      return data.serverTime;
    } catch (error) {
      console.error(error);
    }
  }

(async()=>{

    const timestamp = await getBinanceServerTime();
  
    const queryString = `symbol=${symbol}&side=${side}&type=${type}&quantity=${quantity}&timestamp=${timestamp}`;

    const signature = crypto
    .createHmac('sha256', secretKey)
    .update(queryString)
    .digest('hex');

    const headers = {
    'X-MBX-APIKEY': apiKey,
    };


    axios
    .post(endpoint, `${queryString}&signature=${signature}`, { headers })
    .then((response) => {
      console.log(response.data);
    })
    .catch((error) => {
      console.error(error);
    });




})();


