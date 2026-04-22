package com.softwave.transportsystem;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.context.ConfigurableApplicationContext;

@SpringBootApplication
public class TransportSystemServerApplication {

	public static void main(String[] args) {
		ConfigurableApplicationContext context = SpringApplication.run(TransportSystemServerApplication.class, args);
		// Retrieve the HomeController bean from the application context

	}

}
