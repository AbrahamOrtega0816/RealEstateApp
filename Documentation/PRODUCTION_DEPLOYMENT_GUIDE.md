# 🚀 Production Deployment Guide - Real Estate API

## 🔧 Railway Backend Configuration

### Environment Variables Required on Railway

Set these environment variables in your Railway project dashboard:

```bash
# MongoDB Configuration
MONGODB_PASSWORD=your_mongodb_atlas_password_here

# CORS Configuration - CRITICAL for frontend access
ALLOWED_ORIGINS=https://real-estate-app-kappa-six.vercel.app,https://your-other-domains.com

# JWT Configuration (optional - uses appsettings.json values if not set)
JWT_SECRET_KEY=your-super-secure-jwt-secret-key-at-least-32-characters

# Optional: Enable Swagger in production
ENABLE_SWAGGER=true

# Optional: Enable HTTPS redirect (only if you have proper SSL setup)
ENABLE_HTTPS_REDIRECT=false
```

### 🌐 CORS Configuration Details

Your backend is configured to allow these origins:

**Default (fallback) origins:**
- `http://localhost:3000` (local development)
- `http://localhost:3001` (local development)
- `https://real-estate-app-kappa-six.vercel.app` (your Vercel frontend)

**Production origins (via ALLOWED_ORIGINS env var):**
- Set `ALLOWED_ORIGINS` to include all your frontend domains
- Separate multiple domains with commas
- Example: `https://real-estate-app-kappa-six.vercel.app,https://your-custom-domain.com`

## 🎯 Vercel Frontend Configuration

### Environment Variables Required on Vercel

Set these in your Vercel project dashboard:

```bash
# Backend API URL
NEXT_PUBLIC_API_URL=https://realestateapp-production-2da3.up.railway.app/api
```

## 🔍 Troubleshooting CORS Issues

### 1. Verify Railway Environment Variables

Check that `ALLOWED_ORIGINS` is set correctly on Railway:

```bash
# Should include your Vercel domain
ALLOWED_ORIGINS=https://real-estate-app-kappa-six.vercel.app
```

### 2. Check Backend Logs

Look for this log message in Railway:
```
🌐 CORS: Allowing origins: https://real-estate-app-kappa-six.vercel.app
```

### 3. Verify Frontend Configuration

Ensure your frontend is pointing to the correct API URL:
- Check `NEXT_PUBLIC_API_URL` in Vercel environment variables
- Should be: `https://realestateapp-production-2da3.up.railway.app/api`

### 4. Test API Endpoints

Test your API directly:
```bash
curl -H "Origin: https://real-estate-app-kappa-six.vercel.app" \
     -H "Access-Control-Request-Method: POST" \
     -H "Access-Control-Request-Headers: Content-Type" \
     -X OPTIONS \
     https://realestateapp-production-2da3.up.railway.app/api/auth/login
```

Should return CORS headers:
```
Access-Control-Allow-Origin: https://real-estate-app-kappa-six.vercel.app
Access-Control-Allow-Methods: GET,HEAD,PUT,PATCH,POST,DELETE
Access-Control-Allow-Headers: Content-Type
```

## 🚨 Common Issues & Solutions

### Issue 1: CORS Error on Login
**Cause:** Frontend domain not in ALLOWED_ORIGINS
**Solution:** Add your Vercel domain to ALLOWED_ORIGINS on Railway

### Issue 2: API Not Found (404)
**Cause:** Incorrect API URL in frontend
**Solution:** Verify NEXT_PUBLIC_API_URL points to your Railway backend

### Issue 3: MongoDB Connection Error
**Cause:** Missing or incorrect MONGODB_PASSWORD
**Solution:** Set correct MongoDB Atlas password on Railway

### Issue 4: JWT Token Issues
**Cause:** Different JWT secrets between environments
**Solution:** Set same JWT_SECRET_KEY on both Railway and Vercel (if needed)

## 📝 Deployment Checklist

### Railway Backend:
- [ ] Set `MONGODB_PASSWORD`
- [ ] Set `ALLOWED_ORIGINS` with your Vercel domain
- [ ] Verify deployment is successful
- [ ] Test API endpoints directly
- [ ] Check logs for CORS configuration

### Vercel Frontend:
- [ ] Set `NEXT_PUBLIC_API_URL` to your Railway backend
- [ ] Verify build is successful
- [ ] Test login functionality
- [ ] Check browser network tab for CORS headers

## 🔄 Quick Fix Commands

If you need to quickly update your Railway environment variables:

```bash
# Using Railway CLI (if installed)
railway variables set ALLOWED_ORIGINS=https://real-estate-app-kappa-six.vercel.app

# Or update through Railway dashboard:
# 1. Go to your Railway project
# 2. Click on "Variables" tab
# 3. Add/Update ALLOWED_ORIGINS
# 4. Redeploy your service
```

## 🎯 Testing Your Fix

1. **Deploy the updated backend** to Railway
2. **Wait for deployment** to complete
3. **Test login** from your Vercel frontend
4. **Check browser console** for any remaining errors
5. **Verify in Network tab** that CORS headers are present

The CORS error should be resolved once you set the `ALLOWED_ORIGINS` environment variable on Railway!
