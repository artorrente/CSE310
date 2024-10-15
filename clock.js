// Where everything starts!
function showTime() {
	// Get the current time
	const time = new Date();
	let hour = time.getHours();
	let min = time.getMinutes();
	let sec = time.getSeconds();
	let am_pm = "AM";
  
	// Converts to 12-hour format
	if (hour >= 12) {
	  if (hour > 12) hour -= 12;
	  am_pm = "PM";
	} else if (hour === 0) {
	  hour = 12;
	  am_pm = "AM";
	}
  
	// Add leading zeros if necessary 08 8
	hour = hour < 10 ? `0${hour}` : hour;
	min = min < 10 ? `0${min}` : min;
	sec = sec < 10 ? `0${sec}` : sec;
  
	// Format the time string
	const currentTime = `${hour}:${min}:${sec} ${am_pm}`;
  
	// Update the clock display
	document.getElementById("clock").innerHTML = currentTime;
  }
  
  // Call the showTime function every second
  setInterval(showTime, 5000); // Note: 1000ms = 1 second
  
  // Initial call to showTime
  showTime();