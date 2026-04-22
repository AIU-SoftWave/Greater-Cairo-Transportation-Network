package com.softwave.transportsystem.repository;

import com.softwave.transportsystem.model.MetroLine;
import org.springframework.data.jpa.repository.JpaRepository;

public interface MetroLineRepository extends JpaRepository<MetroLine, String> {
}
