/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  env: {
    API_BASE_URL: 'http://localhost:5258'
  }
}

module.exports = nextConfig
