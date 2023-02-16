require("dotenv").config();
const axios = require('axios');
const crypto = require('crypto');
const NTPClient = require("ntp-client");

const API_KEY = process.env.API_KEY;
const SECRET_KEY = process.env.SECRET_KEY;
const SYMBOL = 'BTCUSDT';  // replace with the symbol pair you want to retrieve orders for

const headers = new Headers({
  'X-MBX-APIKEY': API_KEY
});

const requestOptions = {
  method: 'GET',
  headers: headers
};



async function getBinanceServerTime() {
    try {
      const response = await fetch('https://testnet.binance.vision/api/v1/time');
      const data = await response.json();
      return data.serverTime;
    } catch (error) {
      console.error(error);
    }
  }

function getSignature(queryString) {
  const hmac = crypto.createHmac('sha256', SECRET_KEY);
  hmac.update(queryString);
  return hmac.digest('hex');
}

async function getOpenOrders() {
  const timestamp = await getBinanceServerTime();
  const queryString = `symbol=${SYMBOL}&timestamp=${timestamp}`;
  const signature = getSignature(queryString);
  const response = await fetch(`https://testnet.binance.vision/api/v3/openOrders?${queryString}&signature=${signature}`, requestOptions);

  const teste = await response.text();
 
  const data = await response.json();
  return data;
}

getOpenOrders().then(console.log);
