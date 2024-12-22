
const mongoose = require('mongoose');
require('dotenv').config();

// Connect to MongoDB mongodb://localhost:27017/?directConnection=true
mongoose.connect(process.env.MONGODB_URI, {
    useNewUrlParser: true,
    useUnifiedTopology: true
})
.then(() => console.log('Connected to MongoDB'))
.catch(err => console.error('Could not connect to MongoDB:', err));

const express = require('express');
const bodyParser = require('body-parser');
const User = require('./Models/user');

// Create an instance of Express
const app = express();

// Middleware to parse JSON bodies
app.use(bodyParser.json());


// Create a new user (POST)
app.post('/users', async (req, res) => {
    // const user = new User({
    //     name: req.body.name,
    //     email: req.body.email,
    //     age: req.body.age,
    //     socialMediaProfiles: req.body.socialMediaProfiles || {},
    //     preferences: req.body.preferences || {}
    // });
    const user = new User(req.body);

    try {
        const result = await user.save();
        res.status(201).json(result);
    } catch (err) {
        res.status(400).send(err.message);
    }
});


// Get all users (GET)
app.get('/users', async (req, res) => {
    try {
        const users = await User.find();
        res.json(users);
    } catch (err) {
        res.status(500).send(err.message);
    }
});

// Get a user by ID (GET)
app.get('/users/:id', async (req, res) => {
    try {
        const user = await User.findById(req.params.id);
        if (!user) return res.status(404).send('User not found.');
        res.json(user);
    } catch (err) {
        res.status(400).send('Invalid ID');
    }
});


// Update a user by ID (PUT)
app.put('/users/:id', async (req, res) => {
    try {
        const user = await User.findByIdAndUpdate(req.params.id, req.body, {
            new: true,
            runValidators: true 
        });
        if (!user) return res.status(404).send('User not found.');
        res.json(user);
    } catch (err) {
        res.status(400).send('Invalid ID');
    }
});


// Delete a user by ID (DELETE)
app.delete('/users/:id', async (req, res) => {
    try {
        const user = await User.findByIdAndDelete(req.params.id);
        if (!user) return res.status(404).send('User not found.');
        res.status(204).send();
    } catch (err) {
        res.status(400).send('Invalid ID');
    }
});


// Start the server
const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
    console.log(`Server is running on port ${PORT}`);
    console.log('MongoDB URI:', process.env.MONGODB_URI);

});
