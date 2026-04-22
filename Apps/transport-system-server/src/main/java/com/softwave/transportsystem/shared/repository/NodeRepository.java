package com.softwave.transportsystem.shared.repository;

import com.softwave.transportsystem.shared.model.AbstractNode;
import org.springframework.data.jpa.repository.JpaRepository;

public interface NodeRepository extends JpaRepository<AbstractNode, String> {
}
